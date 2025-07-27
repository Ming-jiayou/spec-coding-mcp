using System.ComponentModel.DataAnnotations;

namespace SpecCodingMcpServer.Models;

/// <summary>
/// Configuration settings for the SpecCoding MCP server.
/// Defines paths, timeouts, and other operational parameters.
/// </summary>
public class SpecCodingConfiguration
{
    /// <summary>
    /// The configuration section name used in configuration files.
    /// </summary>
    public const string SectionName = "SpecCoding";

    /// <summary>
    /// Gets or sets the path to the directory containing prompt templates.
    /// These prompts are used to generate content at different workflow stages.
    /// </summary>
    [Required]
    public string PromptsPath { get; set; } = "Prompts";

    /// <summary>
    /// Gets or sets the base path for all generated output files.
    /// Files created during workflow execution will be stored in subdirectories under this path.
    /// </summary>
    [Required]
    public string OutputPath { get; set; } = ".spec-coding";

    /// <summary>
    /// Gets or sets the timeout duration for inactive sessions.
    /// Sessions that exceed this timeout without activity will be automatically cleaned up.
    /// </summary>
    [Required]
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets the absolute path to the prompts directory.
    /// Handles both relative and absolute paths by combining with the application base directory if necessary.
    /// </summary>
    /// <returns>The absolute path to the prompts directory.</returns>
    public string GetAbsolutePromptsPath()
    {
        return Path.IsPathRooted(PromptsPath)
            ? PromptsPath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PromptsPath);
    }
}