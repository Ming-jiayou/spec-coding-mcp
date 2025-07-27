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
/// MCP tool for starting the requirements gathering phase.
/// Loads the requirements analysis prompt for the specified feature.
/// </summary>
public class RequirementsStartTool
{
    private readonly ISessionManager _sessionManager;
    private readonly IFileService _fileService;
    private readonly SpecCodingConfiguration _specCodingConfiguration;
    private readonly ILogger<FeatureConfirmedTool> _logger;
    private readonly IContextManager _contextManager;

    /// <summary>
    /// Initializes a new instance of the RequirementsStartTool class.
    /// </summary>
    public RequirementsStartTool(
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
    /// Starts the requirements gathering phase for a feature.
    /// Loads the requirements analysis prompt with feature context.
    /// </summary>
    /// <param name="sessionId">The unique session identifier.</param>
    /// <param name="featureName">The name of the feature being analyzed.</param>
    /// <returns>The requirements analysis prompt with feature context.</returns>
    [McpServerTool]
    [Description("Start the requirements gathering phase")]
    public async Task<string> SpecCodingRequirementsStart(
        [Description("Session Id")] string sessionId,
        [Description("Feature Name based on the confirmed feature")] string featureName)
    {
        try
        {
            Log.Information($"Starting requirements gathering for session  {sessionId}  {featureName}");

            var sessionState = _sessionManager.GetSession(sessionId);
            if (sessionState == null)
            {
                throw new SessionNotFoundException(sessionId);
            }

            if (sessionState.CurrentStage != WorkflowStage.RequirementsGathering)
            {
                throw new WorkflowException(WorkflowStage.RequirementsGathering.ToString(), sessionId);
            }

            var template = await _contextManager.LoadPromptAsync("RequirementsGathering.md");
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
            Log.Fatal(ex, "Error executing requirements start");
            throw new InvalidOperationException("Error executing requirements start", ex);
        }
    }

}