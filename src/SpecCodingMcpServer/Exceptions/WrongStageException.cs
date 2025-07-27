using System;

namespace SpecCodingMcpServer.Exceptions
{
    /// <summary>
    /// Exception thrown when a workflow operation is attempted at an invalid stage.
    /// This ensures proper workflow state management and prevents invalid transitions.
    /// </summary>
    public class WorkflowException : Exception
    {
        /// <summary>
        /// Gets the current workflow stage where the error occurred.
        /// </summary>
        public string WorkflowStage { get; }

        /// <summary>
        /// Gets the session ID associated with the workflow operation.
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// Gets the standardized error code for workflow-related exceptions.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the WorkflowException class with workflow stage and session ID.
        /// </summary>
        /// <param name="workflowStage">The workflow stage where the error occurred.</param>
        /// <param name="sessionId">The session ID associated with the workflow operation.</param>
        public WorkflowException(string workflowStage, string sessionId)
            : base($"Workflow error: {workflowStage} (session: {sessionId})")
        {
            WorkflowStage = workflowStage;
            SessionId = sessionId;
            ErrorCode = "WORKFLOW_ERROR";
        }

        /// <summary>
        /// Initializes a new instance of the WorkflowException class with workflow stage, session ID, and custom message.
        /// </summary>
        /// <param name="workflowStage">The workflow stage where the error occurred.</param>
        /// <param name="sessionId">The session ID associated with the workflow operation.</param>
        /// <param name="message">The custom error message that describes the exception.</param>
        public WorkflowException(string workflowStage, string sessionId, string message)
            : base(message)
        {
            WorkflowStage = workflowStage;
            SessionId = sessionId;
            ErrorCode = "WORKFLOW_ERROR";
        }

        /// <summary>
        /// Initializes a new instance of the WorkflowException class with workflow stage, session ID, custom message, and inner exception.
        /// </summary>
        /// <param name="workflowStage">The workflow stage where the error occurred.</param>
        /// <param name="sessionId">The session ID associated with the workflow operation.</param>
        /// <param name="message">The custom error message that describes the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public WorkflowException(string workflowStage, string sessionId, string message, Exception innerException)
            : base(message, innerException)
        {
            WorkflowStage = workflowStage;
            SessionId = sessionId;
            ErrorCode = "WORKFLOW_ERROR";
        }
    }
}