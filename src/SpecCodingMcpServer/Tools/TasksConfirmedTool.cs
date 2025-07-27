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

public class TasksConfirmedTool
{
    private readonly ISessionManager _sessionManager;
    private readonly IFileService _fileService;
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    private readonly ILogger<FeatureConfirmedTool> _logger;
    private readonly IContextManager _contextManager;

    public TasksConfirmedTool(
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
    [Description("Confirm the completion of the create task list document and proceed to the task exetution phase")]
    public async Task<string> SpecCodingTasksConfirmed(
        [Description("Session Id")] string sessionId,
               [Description("Feature Name based on the confirmed feature")] string featureName)
    {
        try
        {


            Log.Information($"Tasks confirmed for session {sessionId} {featureName}");


            var sessionState = _sessionManager.GetSession(sessionId);
            if (sessionState == null)
            {
                throw new SessionNotFoundException(sessionId);
            }

            if (sessionState.CurrentStage != WorkflowStage.CreateTaskList)
            {
                throw new WorkflowException(WorkflowStage.CreateTaskList.ToString(), sessionId);
            }

            sessionState.CurrentStage = WorkflowStage.TaskExecution;
            sessionState.UpdatedAt = DateTime.UtcNow;
            _sessionManager.UpdateSession(sessionId, sessionState);

            var template = await _contextManager.LoadPromptAsync("TasksConfirmed.md");
            var result = _contextManager.GetPrompt(template, new Dictionary<string, object>
            {
                { "session_id", sessionId},
                { "feature_name", sessionState.FeatureName }
            });

            return result;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error executing tasks confirmation");
            throw new InvalidOperationException("Error executing tasks confirmation", ex);
        }
    } 
}