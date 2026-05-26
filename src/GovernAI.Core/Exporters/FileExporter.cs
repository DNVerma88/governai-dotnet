using System.Text;
using System.Text.Json;
using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// An <see cref="IGovernAIExporter"/> that appends events as JSON Lines to a file.
/// </summary>
/// <remarks>
/// One event is written per line. Thread-safe via a <see cref="SemaphoreSlim"/>.
/// Dispose this instance to release the semaphore when the exporter is no longer needed.
/// </remarks>
public sealed class FileExporter : IGovernAIExporter, IDisposable
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="FileExporter"/> with the specified file path.
    /// </summary>
    /// <param name="filePath">
    /// The absolute path to the JSON Lines output file. Must not be <see langword="null"/>.
    /// Relative paths are rejected to prevent path-traversal attacks.
    /// </param>
    public FileExporter(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or whitespace.", nameof(filePath));
        }

        // Reject relative paths: an attacker who controls configuration could use
        // "../../etc/cron.d/payload" to write to arbitrary system locations (VULN-07).
        if (!Path.IsPathFullyQualified(filePath))
        {
            throw new ArgumentException(
                "File path must be an absolute path. Relative paths are not permitted.",
                nameof(filePath));
        }

        _filePath = Path.GetFullPath(filePath);
    }

    /// <inheritdoc />
    public async ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var line = JsonSerializer.Serialize(aiEvent, GovernAIJsonContext.Default.GovernAIEvent);

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await File.AppendAllTextAsync(
                _filePath,
                line + Environment.NewLine,
                Encoding.UTF8,
                cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore.Dispose();
            _disposed = true;
        }
    }
}
