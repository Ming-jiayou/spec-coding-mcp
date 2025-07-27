using System;

namespace SpecCodingMcpServer.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested session cannot be found in the session manager.
    /// This typically occurs when a client references a session ID that has expired or never existed.
    /// </summary>
    public class SessionNotFoundException : Exception
    {
        /// <summary>
        /// Gets the unique identifier of the session that was not found.
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// Gets the standardized error code for this exception type.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the SessionNotFoundException class with the specified session ID.
        /// </summary>
        /// <param name="sessionId">The ID of the session that was not found.</param>
        public SessionNotFoundException(string sessionId)
            : base($"Session not found: {sessionId}")
        {
            SessionId = sessionId;
            ErrorCode = "SESSION_NOT_FOUND";
        }

        /// <summary>
        /// Initializes a new instance of the SessionNotFoundException class with the specified session ID and custom message.
        /// </summary>
        /// <param name="sessionId">The ID of the session that was not found.</param>
        /// <param name="message">The custom error message that describes the exception.</param>
        public SessionNotFoundException(string sessionId, string message)
            : base(message)
        {
            SessionId = sessionId;
            ErrorCode = "SESSION_NOT_FOUND";
        }

        /// <summary>
        /// Initializes a new instance of the SessionNotFoundException class with the specified session ID, custom message, and inner exception.
        /// </summary>
        /// <param name="sessionId">The ID of the session that was not found.</param>
        /// <param name="message">The custom error message that describes the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public SessionNotFoundException(string sessionId, string message, Exception innerException)
            : base(message, innerException)
        {
            SessionId = sessionId;
            ErrorCode = "SESSION_NOT_FOUND";
        }
    }
}