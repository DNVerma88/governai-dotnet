namespace GovernAI.Abstractions;

/// <summary>
/// Defines the contract for exporting GovernAI governance events.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to send <see cref="GovernAIEvent"/> records to a destination such as
/// the console, a local file, an in-memory store, or a remote GovernAI Collector.
/// </para>
/// <para>
/// Implementations must be thread-safe.
/// Exporter failures must not propagate exceptions to the calling application by default.
/// </para>
/// </remarks>
public interface IGovernAIExporter
{
    /// <summary>
    /// Exports a <see cref="GovernAIEvent"/> asynchronously.
    /// </summary>
    /// <param name="event">The governance event to export. Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous export operation.</returns>
    ValueTask ExportAsync(GovernAIEvent @event, CancellationToken cancellationToken = default);
}
