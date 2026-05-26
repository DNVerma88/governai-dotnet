namespace GovernAI.Abstractions;

/// <summary>
/// Defines the contract for resolving the current tenant identifier.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to provide tenant resolution logic appropriate for your application.
/// Tenant ID can come from HTTP headers, JWT claims, explicit context, or a custom data source.
/// </para>
/// <para>
/// GovernAI does not assume a specific tenant model. Implementations must be thread-safe.
/// </para>
/// </remarks>
public interface IGovernAITenantResolver
{
    /// <summary>
    /// Resolves the current tenant identifier asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing the tenant identifier,
    /// or <see langword="null"/> if the tenant cannot be resolved (e.g., single-tenant applications).
    /// </returns>
    ValueTask<string?> ResolveTenantIdAsync(CancellationToken cancellationToken = default);
}
