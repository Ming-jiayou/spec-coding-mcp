using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using SpecCodingMcpServer.Exceptions;
using SpecCodingMcpServer.Models;
using SpecCodingMcpServer.Services;
using System.ComponentModel;

namespace SpecCodingMcpServer.Tools;

/// <summary>
/// MCP tool for confirming feature definition.
/// Transitions the workflow from feature definition to requirements gathering.
/// </summary>
public class FeatureConfirmedTool
{
    private readonly ISessionManager _sessionManager;
    private readonly IFileService _fileService;
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    private readonly ILogger<FeatureConfirmedTool> _logger;
    private readonly IContextManager _contextManager;

    /// <summary>
    /// Initializes a new instance of the FeatureConfirmedTool class.
    /// </summary>
    public FeatureConfirmedTool(
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
    /// Confirms the feature definition and transitions to requirements gathering.
    /// Updates the session state and loads the requirements prompt.
    /// </summary>
    /// <param name="sessionId">The unique session identifier.</param>
    /// <param name="featureName">The name of the confirmed feature.</param>
    /// <param name="featureDetail">Detailed information about the feature.</param>
    /// <returns>The requirements gathering prompt with feature context.</returns>
    [McpServerTool]
    [Description("Confirm the feature definition and proceed to the requirements phase")]
    public async Task<string> SpecCodingFeatureConfirmed(
        [Description("Session Id")] string sessionId,
        [Description("Feature Name")] string featureName,
        [Description("Feature Detail Information")] string featureDetail)
    {
        Log.Information($"Feature confirmed for session {sessionId} with feature: {featureName}");

        // 验证会话存在并更新状态
        var sessionState = _sessionManager.GetSession(sessionId);
        if (sessionState == null)
        {
            throw new SessionNotFoundException(sessionId);
        }

        sessionState.CurrentStage = WorkflowStage.RequirementsGathering;
        sessionState.UpdatedAt = DateTime.UtcNow;
        _sessionManager.UpdateSession(sessionId, sessionState);
         

        var template = await _contextManager.LoadPromptAsync("FeatureConfirmed.md");
        var result = _contextManager.GetPrompt(template, new Dictionary<string, object>
            {
                { "session_id", sessionId},{"feature_name", featureName }, { "feature_detail", featureDetail}, { "feature_folder", $"{_specCodingConfiguration.OutputPath}/features/{featureName}"}
            });

        return result;
    }
}