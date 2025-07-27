using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using Serilog;
using SpecCodingMcpServer.Models;
using SpecCodingMcpServer.Services;
using System.ComponentModel;
using System.Text;


namespace SpecCodingMcpServer.Tools;

/// <summary>
/// MCP tool for starting the SpecCoding workflow.
/// Initializes a new session and returns the feature definition prompt.
/// </summary>
public class WorkflowStartTool 
{
    // Configuration settings for output paths and operational parameters
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    
    // Service for managing session state and lifecycle
    private readonly ISessionManager _sessionManager;
    
    // Service for loading and processing prompt templates
    private readonly IContextManager _contextManager;
    
    // Logger for diagnostic and error messages
    private readonly ILogger<WorkflowStartTool> _logger;

    /// <summary>
    /// Initializes a new instance of the WorkflowStartTool class.
    /// </summary>
    /// <param name="sessionManager">The session manager for creating and managing sessions.</param>
    /// <param name="contextManager">The context manager for loading and processing templates.</param>
    /// <param name="logger">The logger instance for diagnostic messages.</param>
    public WorkflowStartTool(
        ISessionManager sessionManager,
        IContextManager contextManager,
        ILogger<WorkflowStartTool> logger)
    {
        _specCodingConfiguration = new SpecCodingConfiguration();
        _sessionManager = sessionManager;
        _contextManager = contextManager;
        _logger = logger;
    }

    /// <summary>
    /// Starts a new SpecCoding workflow session.
    /// Creates a new session with FeatureDefine stage and returns the initial prompt.
    /// </summary>
    /// <returns>The feature definition prompt with session information.</returns>
    [McpServerTool]
    [Description("Start Spec Coding Workflow")]
    public async Task<string> SpecCodingWorkflowStart()
    {
        try
        {
            // Generate a new unique session ID for this workflow
            var sessionId = _sessionManager.GenerateSessionId();
            Log.Information("Starting workflow with session_id: {SessionId}", sessionId);

            // Create initial session state with FeatureDefine stage
            var sessionState = new Models.SessionState
            {
                SessionId = sessionId,
                CurrentStage = Models.WorkflowStage.FeatureDefine,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            // Store the session state for tracking
            _sessionManager.UpdateSession(sessionId, sessionState);

            // Load the feature definition prompt template
            var template = await _contextManager.LoadPromptAsync("FeatureDefine.md");
            
            // Process template with session-specific variables
            var result = _contextManager.GetPrompt(template, new Dictionary<string, object>
            {
                { "session_id", sessionId }, 
                { "features_folder", $"{_specCodingConfiguration.OutputPath}/features" }
            }); 

            return result;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error executing workflow start");
            throw new InvalidOperationException("Error executing workflow start", ex);
        }
    }
}