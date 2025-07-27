using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using Serilog;
using SpecCodingMcpServer.Exceptions;
using SpecCodingMcpServer.Models;
using SpecCodingMcpServer.Services;
using System.ComponentModel;

namespace SpecCodingMcpServer.Tools;

public class TasksStartTool
{
    private readonly ISessionManager _sessionManager;
    private readonly IFileService _fileService;
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    private readonly ILogger<FeatureConfirmedTool> _logger;
    private readonly IContextManager _contextManager;

    public TasksStartTool(
        ISessionManager sessionManager,
        IFileService fileService,
                IContextManager contextManager,
         
        ILogger<FeatureConfirmedTool> logger)
    {
        _sessionManager = sessionManager;
        _fileService = fileService;
       _specCodingConfiguration = new SpecCodingConfiguration();
        _logger= logger;
        _contextManager=contextManager;
    }

    [McpServerTool]
    [Description("Start the create task list phase")]
    public async Task<string> SpecCodingTasksStart(
        [Description("Session Id")] string sessionId,
        [Description("Feature Name based on the confirmed feature")] string featureName)
    {
        try
        {
            Log.Information($"Starting  create task list phase for session  {sessionId}  {featureName}");

            var sessionState = _sessionManager.GetSession(sessionId);
            if (sessionState == null)
            {
                throw new SessionNotFoundException(sessionId);
            }

            if (sessionState.CurrentStage != WorkflowStage.CreateTaskList)
            {
                throw new WorkflowException(WorkflowStage.CreateTaskList.ToString(), sessionId);
            }

            var template = await _contextManager.LoadPromptAsync("CreateTaskList.md");
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
            Log.Fatal(ex, "Error executing create task list start");
            throw new InvalidOperationException("Error executing create task list start", ex);
        }
    }

}