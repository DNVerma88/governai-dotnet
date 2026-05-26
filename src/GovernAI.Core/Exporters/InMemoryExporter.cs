using System.Collections.Concurrent;
using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// An <see cref="IGovernAIExporter"/> that stores events in memory with a configurable capacity bound.
/// </summary>
/// <remarks>
/// Oldest events are evicted when the capacity is exceeded. Thread-safe.
/// </remarks>
public sealed class InMemoryExporter : IGovernAIExporter
{
    private readonly ConcurrentQueue<GovernAIEvent> _queue = new();
    private readonly int _capacity;
    private readonly object _trimLock = new();

    /// <summary>
    /// Initializes a new instance of <see cref="InMemoryExporter"/> with the specified capacity.
    /// </summary>
    /// <param name="capacity">Maximum number of events to retain. Must be greater than zero.</param>
    public InMemoryExporter(int capacity = 1000)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
        }

        _capacity = capacity;
    }

    /// <summary>
    /// Gets a snapshot of all stored events.
    /// </summary>
    public IReadOnlyList<GovernAIEvent> Events => _queue.ToArray();

    /// <inheritdoc />
    public ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default)
    {
        // Lock ensures the enqueue + trim pair is atomic, preventing a TOCTOU race
        // where concurrent callers could over-fill or over-trim the queue (VULN-13).
        lock (_trimLock)
        {
            _queue.Enqueue(aiEvent);

            while (_queue.Count > _capacity)
            {
                _queue.TryDequeue(out _);
            }
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Removes all stored events.
    /// </summary>
    public void Clear()
    {
        while (_queue.TryDequeue(out _))
        {
        }
    }
}
