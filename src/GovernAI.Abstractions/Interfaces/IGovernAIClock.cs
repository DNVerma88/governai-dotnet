namespace GovernAI.Abstractions;

/// <summary>
/// Defines the contract for providing the current UTC time.
/// </summary>
/// <remarks>
/// <para>
/// This abstraction enables deterministic unit testing of time-sensitive SDK components.
/// </para>
/// <para>
/// The default implementation returns <see cref="DateTimeOffset.UtcNow"/>.
/// Test implementations can return a fixed or controlled timestamp.
/// </para>
/// </remarks>
public interface IGovernAIClock
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
