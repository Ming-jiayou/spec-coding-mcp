using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCodingMcpServer.Services
{
    /// <summary>
    /// Defines the contract for file operations in the SpecCoding MCP server.
    /// Provides methods for reading and writing files with proper error handling and logging.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Asynchronously writes content to a file at the specified path.
        /// </summary>
        /// <param name="path">The absolute or relative path to the file.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <returns>A task representing the asynchronous write operation.</returns>
        /// <exception cref="ArgumentException">Thrown when path is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when content is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when file write fails.</exception>
        Task WriteFileAsync(string path, string content);

        /// <summary>
        /// Asynchronously reads the content of a file at the specified path.
        /// </summary>
        /// <param name="path">The absolute or relative path to the file.</param>
        /// <returns>A task containing the file content as a string.</returns>
        /// <exception cref="ArgumentException">Thrown when path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when file read fails.</exception>
        Task<string> ReadFileAsync(string path);
    }

    /// <summary>
    /// Provides file system operations with logging and error handling.
    /// Uses UTF-8 encoding for all file operations and provides detailed logging.
    /// </summary>
    internal class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        /// <summary>
        /// Initializes a new instance of the FileService class.
        /// </summary>
        /// <param name="logger">The logger instance for diagnostic messages.</param>
        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously writes content to a file with UTF-8 encoding.
        /// Creates directories as needed and logs operation details.
        /// </summary>
        /// <param name="path">The absolute or relative path to the file.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <returns>A task representing the asynchronous write operation.</returns>
        public async Task WriteFileAsync(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            try
            {
                await File.WriteAllTextAsync(path, content, Encoding.UTF8);
                _logger.LogDebug("Wrote file: {Path}, Size: {Size} bytes", path, content.Length);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unexpected error writing to file: {Path}", path);
                throw new InvalidOperationException($"Failed to write file: {path}", ex);
            }
        }

        /// <summary>
        /// Asynchronously reads the content of a file with UTF-8 encoding.
        /// Validates file existence and logs operation details.
        /// </summary>
        /// <param name="path">The absolute or relative path to the file.</param>
        /// <returns>A task containing the file content as a string.</returns>
        public async Task<string> ReadFileAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogWarning("File not found: {Path}", path);
                    throw new FileNotFoundException($"File not found: {path}");
                }

                var content = await File.ReadAllTextAsync(path, Encoding.UTF8);
                _logger.LogDebug("Read file: {Path}, Size: {Size} bytes", path, content.Length);
                return content;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unexpected error reading file: {Path}", path);
                throw new InvalidOperationException($"Failed to read file: {path}", ex);
            }
        }

    }
}
