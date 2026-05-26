namespace GovernAI.Abstractions;

/// <summary>
/// Defines the contract for evaluating AI governance policies.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to provide custom policy logic. The MVP includes a local
/// default evaluator. Future versions may support a remote GovernAI Policy Server.
/// </para>
/// <para>
/// Implementations must be thread-safe and must not throw exceptions that crash the host application.
/// </para>
/// </remarks>
public interface IGovernAIPolicyEvaluator
{
    /// <summary>
    /// Evaluates the governance policy for the given operation context asynchronously.
    /// </summary>
    /// <param name="context">The context describing the AI operation to evaluate. Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing the <see cref="GovernAIPolicyDecision"/>
    /// with the policy decision, risk score, risk level, and reason.
    /// </returns>
    ValueTask<GovernAIPolicyDecision> EvaluateAsync(GovernAIContext context, CancellationToken cancellationToken = default);
}
