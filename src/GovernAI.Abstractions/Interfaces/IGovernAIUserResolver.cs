namespace GovernAI.Abstractions;

/// <summary>
/// Defines the contract for resolving the current user identifier.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to provide user resolution logic appropriate for your application.
/// User ID is typically resolved from JWT claims or a custom identity provider.
/// </para>
/// <para>
/// Implementations must not store full user profile data.
/// Implementations must be thread-safe.
/// </para>
/// </remarks>
public interface IGovernAIUserResolver
{
    /// <summary>
    /// Resolves the current user identifier asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing the user identifier,
    /// or <see langword="null"/> if the user cannot be resolved (e.g., system or background operations).
    /// </returns>
    ValueTask<string?> ResolveUserIdAsync(CancellationToken cancellationToken = default);
}
