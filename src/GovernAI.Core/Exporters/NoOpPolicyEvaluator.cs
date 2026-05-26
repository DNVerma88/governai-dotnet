using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// A no-op implementation of <see cref="IGovernAIPolicyEvaluator"/> that always allows operations.
/// </summary>
/// <remarks>
/// Used as the safe default when no policy evaluator is configured.
/// </remarks>
public sealed class NoOpPolicyEvaluator : IGovernAIPolicyEvaluator
{
    /// <summary>
    /// Gets a singleton instance of <see cref="NoOpPolicyEvaluator"/>.
    /// </summary>
    public static readonly NoOpPolicyEvaluator Instance = new();

    /// <inheritdoc />
    public ValueTask<GovernAIPolicyDecision> EvaluateAsync(
        GovernAIContext context,
        CancellationToken cancellationToken = default)
    {
        var decision = new GovernAIPolicyDecision
        {
            Decision = GovernAIPolicyDecisionType.Allow,
            RiskScore = 0,
            RiskLevel = GovernAIRiskLevel.None,
            RiskCategory = null,
            Reason = "No policy evaluator configured"
        };

        return ValueTask.FromResult(decision);
    }
}
