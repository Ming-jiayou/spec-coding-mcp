using Microsoft.Extensions.Configuration;
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
/// MCP tool for confirming design document completion.
/// Transitions the workflow from design document creation to task list creation.
/// </summary>
public class DesignConfirmedTool
{
    // Service for managing session state and validation
    private readonly ISessionManager _sessionManager;
    
    // Service for file operations (unused in this tool but injected)
    private readonly IFileService _fileService;
    
    // Configuration settings for output paths
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    
    // Logger for diagnostic and error messages
    private readonly ILogger<FeatureConfirmedTool> _logger;
    
    // Service for loading and processing prompt templates
    private readonly IContextManager _contextManager;

    /// <summary>
    /// Initializes a new instance of the DesignConfirmedTool class.
    /// </summary>
    /// <param name="sessionManager">The session manager for validating and updating sessions.</param>
    /// <param name="fileService">The file service for file operations.</param>
    /// <param name="contextManager">The context manager for loading templates.</param>
    /// <param name="logger">The logger instance for diagnostic messages.</param>
    public DesignConfirmedTool(
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
    /// Confirms the completion of the design document and transitions to task list creation.
    /// Validates the current workflow stage and updates the session state.
    /// </summary>
    /// <param name="sessionId">The unique session identifier for the workflow.</param>
    /// <param name="featureName">The name of the feature being designed (currently unused).</param>
    /// <returns>The task creation prompt template with session context.</returns>
    [McpServerTool]
    [Description("Confirm the completion of the design document and proceed to the create task list phase")]
    public async Task<string> SpecCodingDesignConfirmed(
        [Description("Session Id")] string sessionId,
        [Description("Feature Name based on the confirmed feature")] string featureName)
    {
        try
        {
            // Log the design confirmation event
            Log.Information($"Design confirmed for session {sessionId} {featureName}");

            // Retrieve the current session state
            var sessionState = _sessionManager.GetSession(sessionId);
            if (sessionState == null)
            {
                throw new SessionNotFoundException(sessionId);
            }

            // Validate that we're in the correct workflow stage
            if (sessionState.CurrentStage != WorkflowStage.CreateFeatureDesignDocument)
            {
                throw new WorkflowException(WorkflowStage.CreateFeatureDesignDocument.ToString(), sessionId);
            }

            // Transition to the next workflow stage
            sessionState.CurrentStage = WorkflowStage.CreateTaskList;
            sessionState.UpdatedAt = DateTime.UtcNow;
            _sessionManager.UpdateSession(sessionId, sessionState);

            // Load the task creation prompt template
            var template = await _contextManager.LoadPromptAsync("FeatureDesignDocumentConfirmed.md");
            
            // Process template with session context
            var result = _contextManager.GetPrompt(template, new Dictionary<string, object>
            {
                { "session_id", sessionId},
                { "feature_name", sessionState.FeatureName }
            });

            return result;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error executing requirements confirmation");
            throw new InvalidOperationException("Error executing requirements confirmation", ex);
        }
    }
     
}