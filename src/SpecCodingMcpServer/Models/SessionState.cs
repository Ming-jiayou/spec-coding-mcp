using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCodingMcpServer.Models
{
    /// <summary>
    /// Defines the different stages in the software development workflow managed by this MCP server.
    /// These stages represent a linear progression from feature definition through task execution.
    /// </summary>
    public enum WorkflowStage
    {
        /// <summary>
        /// Initial stage where the feature concept and basic requirements are defined.
        /// </summary>
        FeatureDefine,

        /// <summary>
        /// Stage where detailed requirements are gathered and documented.
        /// </summary>
        RequirementsGathering,

        /// <summary>
        /// Stage where a comprehensive design document is created based on the requirements.
        /// </summary>
        CreateFeatureDesignDocument,

        /// <summary>
        /// Stage where the feature is broken down into manageable tasks.
        /// </summary>
        CreateTaskList,

        /// <summary>
        /// Final stage where individual tasks are executed to implement the feature.
        /// </summary>
        TaskExecution
    }

    /// <summary>
    /// Represents the state of a single development session managed by the MCP server.
    /// Tracks the progress of a feature from initial definition through implementation.
    /// </summary>
    public class SessionState
    {
        /// <summary>
        /// Gets or sets the unique identifier for this session.
        /// Used to track and manage session state across multiple requests.
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the feature being developed in this session.
        /// </summary>
        public string FeatureName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a brief summary or description of the feature.
        /// </summary>
        public string FeatureSummary { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current stage of the development workflow.
        /// Defaults to FeatureDefine for new sessions.
        /// </summary>
        public WorkflowStage CurrentStage { get; set; } = WorkflowStage.FeatureDefine;

        /// <summary>
        /// Gets or sets the UTC timestamp when this session was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the UTC timestamp when this session was last updated.
        /// Updated whenever session state changes.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
