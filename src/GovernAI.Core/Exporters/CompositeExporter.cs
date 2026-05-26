using GovernAI.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// An <see cref="IGovernAIExporter"/> that fans out export calls to multiple exporters.
/// </summary>
public sealed class CompositeExporter : IGovernAIExporter
{
    private readonly IReadOnlyList<IGovernAIExporter> _exporters;
    private readonly bool _failOnExporterError;
    private readonly ILogger<CompositeExporter> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CompositeExporter"/> with the specified exporters.
    /// </summary>
    /// <param name="exporters">The exporters to fan out to. Must not be <see langword="null"/>.</param>
    /// <param name="failOnExporterError">
    /// When <see langword="true"/>, re-throws the first exporter exception encountered.
    /// When <see langword="false"/>, exceptions are logged and remaining exporters still execute.
    /// </param>
    /// <param name="logger">Optional logger for recording child-exporter failures.</param>
    public CompositeExporter(
        IEnumerable<IGovernAIExporter> exporters,
        bool failOnExporterError = false,
        ILogger<CompositeExporter>? logger = null)
    {
        _exporters = exporters?.ToList() ?? throw new ArgumentNullException(nameof(exporters));
        _failOnExporterError = failOnExporterError;
        _logger = logger ?? NullLogger<CompositeExporter>.Instance;
    }

    /// <inheritdoc />
    public async ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default)
    {
        foreach (var exporter in _exporters)
        {
            try
            {
                await exporter.ExportAsync(aiEvent, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!_failOnExporterError)
            {
                // Log per-child failures so operators can detect partial audit-trail gaps.
                _logger.LogError(ex,
                    "CompositeExporter: child exporter {ExporterType} failed to export event {EventId}.",
                    exporter.GetType().Name, aiEvent.EventId);
            }
        }
    }
}
