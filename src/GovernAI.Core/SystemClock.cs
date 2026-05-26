using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// Default implementation of <see cref="IGovernAIClock"/> backed by <see cref="DateTimeOffset.UtcNow"/>.
/// </summary>
public sealed class SystemClock : IGovernAIClock
{
    /// <summary>
    /// Gets a singleton instance of <see cref="SystemClock"/>.
    /// </summary>
    public static readonly SystemClock Instance = new();

    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
