using GovernAI.Abstractions;

namespace GovernAI.Security;

/// <summary>
/// A local, heuristic-based policy evaluator that combines PII and prompt injection scanning.
/// </summary>
/// <remarks>
/// Policy behavior based on risk level:
/// <list type="bullet">
/// <item><see cref="GovernAIRiskLevel.None"/> → <see cref="GovernAIPolicyDecisionType.Allow"/></item>
/// <item><see cref="GovernAIRiskLevel.Low"/> → <see cref="GovernAIPolicyDecisionType.Allow"/></item>
/// <item><see cref="GovernAIRiskLevel.Medium"/> → <see cref="GovernAIPolicyDecisionType.Allow"/></item>
/// <item><see cref="GovernAIRiskLevel.High"/> → <see cref="GovernAIPolicyDecisionType.Review"/></item>
/// <item><see cref="GovernAIRiskLevel.Critical"/> → <see cref="GovernAIPolicyDecisionType.Deny"/></item>
/// </list>
/// </remarks>
public sealed class DefaultLocalPolicyEvaluator : IGovernAIPolicyEvaluator
{
    private readonly SensitiveDataScanner _sensitiveDataScanner;
    private readonly PromptInjectionHeuristicScanner _promptInjectionScanner;

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultLocalPolicyEvaluator"/>.
    /// </summary>
    public DefaultLocalPolicyEvaluator()
    {
        _sensitiveDataScanner = new SensitiveDataScanner();
        _promptInjectionScanner = new PromptInjectionHeuristicScanner();
    }

    /// <inheritdoc />
    public ValueTask<GovernAIPolicyDecision> EvaluateAsync(
        GovernAIContext context,
        CancellationToken cancellationToken = default)
    {
        var prompt = context.Prompt;

        var sensitiveResult = _sensitiveDataScanner.Scan(prompt);
        var injectionResult = _promptInjectionScanner.Scan(prompt);

        var combined = RiskScoreCalculator.Combine([sensitiveResult, injectionResult]);

        var policyDecision = combined.RiskLevel switch
        {
            GovernAIRiskLevel.Critical => GovernAIPolicyDecisionType.Deny,
            GovernAIRiskLevel.High => GovernAIPolicyDecisionType.Review,
            _ => GovernAIPolicyDecisionType.Allow
        };

        var decision = new GovernAIPolicyDecision
        {
            Decision = policyDecision,
            RiskScore = combined.RiskScore,
            RiskLevel = combined.RiskLevel,
            RiskCategory = combined.RiskCategory,
            Reason = combined.Reason
        };

        return ValueTask.FromResult(decision);
    }
}
