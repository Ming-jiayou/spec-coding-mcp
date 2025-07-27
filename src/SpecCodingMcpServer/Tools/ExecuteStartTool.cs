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
/// MCP tool for starting the task execution phase.
/// Loads the task execution prompt for implementing features.
/// </summary>
public class ExecuteStartTool
{
    private readonly ISessionManager _sessionManager;
    private readonly IFileService _fileService;
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    private readonly ILogger<FeatureConfirmedTool> _logger;
    private readonly IContextManager _contextManager;

    /// <summary>
    /// Initializes a new instance of the ExecuteStartTool class.
    /// </summary>
    public ExecuteStartTool(
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
    /// Starts the task execution phase for implementing features.
    /// Loads the task execution prompt with feature context.
    /// </summary>
    /// <param name="sessionId">The unique session identifier.</param>
    /// <param name="featureName">The name of the feature being implemented.</param>
    /// <returns>The task execution prompt with implementation guidance.</returns>
    [McpServerTool]
    [Description("Execut a specific task from the implementation plan and marking it as complete when finished.")]
    public async Task<string> SpecCodingExecuteStart(
        [Description("Session Id")] string sessionId,
        [Description("Feature Name based on the confirmed feature")] string featureName)
    {
        try
        {
            Log.Information($"Starting  design phase for session  {sessionId}  {featureName}");

            var sessionState = _sessionManager.GetSession(sessionId);
            if (sessionState == null)
            {
                throw new SessionNotFoundException(sessionId);
            }

            if (sessionState.CurrentStage != WorkflowStage.TaskExecution)
            {
                throw new WorkflowException(WorkflowStage.TaskExecution.ToString(), sessionId);
            }

            var template = await _contextManager.LoadPromptAsync("TaskExecution.md");
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
            Log.Fatal(ex, "Error executing task execution start");
            throw new InvalidOperationException("Error executing task execution start", ex);
        }
    }

}