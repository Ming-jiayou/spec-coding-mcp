using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SpecCodingMcpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCodingMcpServer.Services
{
    /// <summary>
    /// Defines the contract for managing user sessions in the SpecCoding MCP server.
    /// Provides methods for session creation, retrieval, and updates.
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Generates a new unique session identifier.
        /// </summary>
        /// <returns>A unique session ID string.</returns>
        string GenerateSessionId();

        /// <summary>
        /// Retrieves the session state for the specified session ID.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session.</param>
        /// <returns>The session state object.</returns>
        /// <exception cref="ArgumentException">Thrown when sessionId is null or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when session is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown when session has expired.</exception>
        SessionState GetSession(string sessionId);

        /// <summary>
        /// Updates the session state for the specified session ID.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session.</param>
        /// <param name="state">The updated session state.</param>
        /// <exception cref="ArgumentException">Thrown when sessionId is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when state is null.</exception>
        void UpdateSession(string sessionId, SessionState state);
    }

    /// <summary>
    /// Manages user sessions for the SpecCoding MCP server.
    /// Provides in-memory session storage with automatic expiration and cleanup.
    /// Implements IDisposable for proper resource cleanup.
    /// </summary>
    public class SessionManager : ISessionManager
    {
        // Configuration settings for the session manager
        private readonly SpecCodingConfiguration _specCodingConfiguration;

        // In-memory storage for active sessions
        private readonly Dictionary<string, SessionState> _sessions;

        // Logger for diagnostic and error messages
        private readonly ILogger<SessionManager> _logger;

        // Timeout duration for session expiration
        private readonly TimeSpan _sessionTimeout;

        // Timer for periodic cleanup of expired sessions
        private readonly Timer _cleanupTimer;

        /// <summary>
        /// Initializes a new instance of the SessionManager class.
        /// Sets up session storage and starts the cleanup timer for expired sessions.
        /// </summary>
        /// <param name="logger">The logger instance for diagnostic messages.</param>
        public SessionManager(ILogger<SessionManager> logger)
        {
            _specCodingConfiguration = new SpecCodingConfiguration();
            _sessions = new Dictionary<string, SessionState>();
            _logger = logger;

            _sessionTimeout = _specCodingConfiguration.SessionTimeout;

            _cleanupTimer = new Timer(CleanupExpiredSessions, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// Generates a new unique session identifier using GUID.
        /// </summary>
        /// <returns>A 32-character hexadecimal string without dashes.</returns>
        public string GenerateSessionId()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Retrieves the session state for the specified session ID.
        /// Automatically checks for session expiration and removes expired sessions.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session to retrieve.</param>
        /// <returns>The session state object if found and not expired.</returns>
        /// <exception cref="ArgumentException">Thrown when sessionId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when session has expired.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when session is not found.</exception>
        public SessionState GetSession(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
            }

            if (_sessions.TryGetValue(sessionId, out var session))
            {
                if (DateTime.UtcNow - session.UpdatedAt > _sessionTimeout)
                {
                    _sessions.Remove(sessionId);
                    _logger.LogWarning("[Session expired: {SessionId}", sessionId);
                    throw new InvalidOperationException($"Session expired: {sessionId}");
                }

                _logger.LogDebug("[Retrieved session: {SessionId}", sessionId);
                return session;
            }

            _logger.LogWarning("[Session not found: {SessionId}", sessionId);
            throw new KeyNotFoundException($"Session not found: {sessionId}");
        }

        /// <summary>
        /// Updates or creates a session with the specified state.
        /// Automatically updates the UpdatedAt timestamp to current UTC time.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session to update.</param>
        /// <param name="state">The session state object containing updated information.</param>
        /// <exception cref="ArgumentException">Thrown when sessionId is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when state is null.</exception>
        public void UpdateSession(string sessionId, SessionState state)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
            }

            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            state.SessionId = sessionId;
            state.UpdatedAt = DateTime.UtcNow;

            _sessions[sessionId] = state;
            _logger.LogDebug("[Updated session: {SessionId}, Stage: {Stage}",
                sessionId, state.CurrentStage);
        }

        /// <summary>
        /// Periodically cleans up expired sessions based on the configured timeout.
        /// Runs as a background task every 10 minutes to remove inactive sessions.
        /// </summary>
        /// <param name="state">Timer state (not used).</param>
        private void CleanupExpiredSessions(object? state)
        {
            try
            {
                var expiredSessions = _sessions
                    .Where(kvp => DateTime.UtcNow - kvp.Value.UpdatedAt > _sessionTimeout)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var sessionId in expiredSessions)
                {
                    _sessions.Remove(sessionId);
                    Log.Information("[Cleaned up expired session: {SessionId}", sessionId);
                }

                if (expiredSessions.Count > 0)
                {
                    Log.Information("[Cleaned up {Count} expired sessions", expiredSessions.Count);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[Error during session cleanup");
            }
        }

        /// <summary>
        /// Disposes of resources used by the session manager.
        /// Specifically stops the cleanup timer to prevent resource leaks.
        /// </summary>
        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }
}
