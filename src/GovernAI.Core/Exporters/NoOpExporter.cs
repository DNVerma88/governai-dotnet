using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// A no-op implementation of <see cref="IGovernAIExporter"/> that discards all events.
/// </summary>
public sealed class NoOpExporter : IGovernAIExporter
{
    /// <summary>
    /// Gets a singleton instance of <see cref="NoOpExporter"/>.
    /// </summary>
    public static readonly NoOpExporter Instance = new();

    /// <inheritdoc />
    public ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default)
        => ValueTask.CompletedTask;
}
