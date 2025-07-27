using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SpecCodingMcpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpecCodingMcpServer.Services
{
    /// <summary>
    /// Defines the contract for managing context and prompt templates.
    /// Provides methods for loading prompt templates and processing them with variable substitution.
    /// </summary>
    public interface IContextManager
    {
        /// <summary>
        /// Asynchronously loads a prompt template from the prompts directory.
        /// </summary>
        /// <param name="templateName">The name of the template file to load.</param>
        /// <returns>A task containing the template content as a string.</returns>
        Task<string> LoadPromptAsync(string templateName);

        /// <summary>
        /// Processes a template by replacing variable placeholders with actual values.
        /// </summary>
        /// <param name="template">The template content with placeholders in {{variable}} format.</param>
        /// <param name="variables">Dictionary of variable names and their corresponding values.</param>
        /// <returns>The processed template with all variables replaced.</returns>
        string GetPrompt(string template, Dictionary<string, object> variables);
    }

    /// <summary>
    /// Manages prompt templates and context processing for the SpecCoding MCP server.
    /// Provides template loading with caching and variable substitution capabilities.
    /// </summary>
    internal class ContextManager : IContextManager
    {
        private readonly IFileService _fileService;
        private readonly SpecCodingConfiguration _specCodingConfiguration;
        private readonly ILogger<ContextManager> _logger;
        private readonly Dictionary<string, string> _templateCache;

        /// <summary>
        /// Initializes a new instance of the ContextManager class.
        /// </summary>
        /// <param name="fileService">The file service for reading template files.</param>
        /// <param name="logger">The logger instance for diagnostic messages.</param>
        public ContextManager(IFileService fileService, ILogger<ContextManager> logger)
        {
            _fileService = fileService;
           _specCodingConfiguration = new SpecCodingConfiguration();
            _logger = logger;
            _templateCache = new Dictionary<string, string>();
        }

        /// <summary>
        /// Asynchronously loads a prompt template from the prompts directory with caching.
        /// Templates are cached in memory to improve performance for repeated loads.
        /// </summary>
        /// <param name="templateName">The name of the template file to load.</param>
        /// <returns>A task containing the template content as a string.</returns>
        /// <exception cref="ArgumentException">Thrown when templateName is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when template loading fails.</exception>
        public async Task<string> LoadPromptAsync(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentException("Template name cannot be null or empty", nameof(templateName));
            }

            try
            {
                // Check cache first to avoid file I/O for previously loaded templates
                if (_templateCache.TryGetValue(templateName, out var cachedTemplate))
                {
                    _logger.LogDebug("Retrieved template from cache: {TemplateName}", templateName);
                    return cachedTemplate;
                }

                // Construct full path to template file
                var templatePath = Path.Combine(_specCodingConfiguration.GetAbsolutePromptsPath(), templateName);

                // Load template from file system
                var template = await _fileService.ReadFileAsync(templatePath);

                // Cache the loaded template for future use
                _templateCache[templateName] = template;

                _logger.LogDebug("Loaded template: {TemplateName}", templateName);
                return template;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to load template: {TemplateName}", templateName);
                throw new InvalidOperationException($"Failed to load template: {templateName}", ex);
            }
        }

        /// <summary>
        /// Processes a template by replacing variable placeholders with actual values.
        /// Supports placeholder format: {{variableName}}.
        /// Logs unprocessed placeholders as warnings.
        /// </summary>
        /// <param name="template">The template content with placeholders in {{variable}} format.</param>
        /// <param name="variables">Dictionary of variable names and their corresponding values.</param>
        /// <returns>The processed template with all variables replaced.</returns>
        /// <exception cref="ArgumentNullException">Thrown when template or variables is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when template processing fails.</exception>
        public string GetPrompt(string template, Dictionary<string, object> variables)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            try
            {
                var result = template;

                // Replace all variable placeholders with actual values
                foreach (var variable in variables)
                {
                    var placeholder = $"{{{{{variable.Key}}}}}";
                    var value = variable.Value?.ToString() ?? string.Empty;
                    result = result.Replace(placeholder, value);

                    _logger.LogDebug("Replaced variable: {Variable} = {Value}",
                        variable.Key, value);
                }

                // Check for any unprocessed placeholders that weren't provided in variables
                var unprocessedPlaceholders = Regex.Matches(result, @"\{\{([^}]+)\}\}")
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .Distinct()
                    .ToList();

                if (unprocessedPlaceholders.Any())
                {
                    _logger.LogWarning("Unprocessed placeholders found: {Placeholders}",
                        string.Join(", ", unprocessedPlaceholders));
                }

                _logger.LogDebug("Template processed successfully, result length: {Length}",
                    result.Length);

                return result;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to process template");
                throw new InvalidOperationException("Failed to process template", ex);
            }
        }

    }
}
