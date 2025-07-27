using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using Serilog;
using SpecCodingMcpServer.Exceptions;
using SpecCodingMcpServer.Models;
using SpecCodingMcpServer.Services;
using System.ComponentModel;

namespace SpecCodingMcpServer.Tools;

/// <summary>
/// MCP tool for starting the design document creation phase.
/// Loads the design document creation prompt for the specified feature.
/// </summary>
public class DesignStartTool
{
    private readonly ISessionManager _sessionManager;
    private readonly IFileService _fileService;
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    private readonly ILogger<FeatureConfirmedTool> _logger;
    private readonly IContextManager _contextManager;

    /// <summary>
    /// Initializes a new instance of the DesignStartTool class.
    /// </summary>
    public DesignStartTool(
        ISessionManager sessionManager,
        IFileService fileService,
        IContextManager contextManager,
        ILogger<FeatureConfirmedTool> logger)
    {
        _sessionManager = sessionManager;
        _fileService = fileService;
        _specCodingConfiguration = new SpecCodingConfiguration();
        _logger = logger;
        _contextManager = contextManager;
    }

    /// <summary>
    /// Starts the design document creation phase for a feature.
    /// Loads the design document creation prompt with feature context.
    /// </summary>
    /// <param name="sessionId">The unique session identifier.</param>
    /// <param name="featureName">The name of the feature being designed.</param>
    /// <returns>The design document creation prompt with feature context.</returns>
    [McpServerTool]
    [Description("Start the design phase")]
    public async Task<string> SpecCodingDesignStart(
        [Description("Session Id")] string sessionId,
        [Description("Feature Name based on the confirmed feature")] string featureName)
    {
        try
        {
            Log.Information($"Starting design phase for session {sessionId} {featureName}");

            // Validate session exists
            var sessionState = _sessionManager.GetSession(sessionId);
            if (sessionState == null)
            {
                throw new SessionNotFoundException(sessionId);
            }

            // Ensure we're in the correct workflow stage
            if (sessionState.CurrentStage != WorkflowStage.CreateFeatureDesignDocument)
            {
                throw new WorkflowException(WorkflowStage.CreateFeatureDesignDocument.ToString(), sessionId);
            }

            // Load the design document creation prompt
            var template = await _contextManager.LoadPromptAsync("CreateFeatureDesignDocument.md");

            // Process template with feature-specific context
            var result = _contextManager.GetPrompt(template, new Dictionary<string, object>
            {
                { "session_id", sessionState.SessionId },
                { "feature_name", sessionState.FeatureName },
                { "feature_folder", $"{_specCodingConfiguration.OutputPath}/features/{featureName}" }
            });
            return result;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error executing design start");
            throw new InvalidOperationException("Error executing design start", ex);
        }
    }

}