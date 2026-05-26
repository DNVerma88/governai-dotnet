namespace GovernAI.Abstractions;

/// <summary>
/// Represents the result of a policy evaluation performed by an <see cref="IGovernAIPolicyEvaluator"/>.
/// </summary>
public sealed record GovernAIPolicyDecision
{
    /// <summary>
    /// Gets the policy decision for the evaluated operation.
    /// </summary>
    public required GovernAIPolicyDecisionType Decision { get; init; }

    /// <summary>
    /// Gets the human-readable reason for the policy decision.
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// Gets the numeric risk score in the range 0–100.
    /// </summary>
    public int RiskScore { get; init; }

    /// <summary>
    /// Gets the risk classification level.
    /// </summary>
    public GovernAIRiskLevel RiskLevel { get; init; }

    /// <summary>
    /// Gets the human-readable risk category (e.g., <c>PromptInjection</c>, <c>SensitiveData</c>).
    /// </summary>
    public string? RiskCategory { get; init; }

    /// <summary>
    /// Gets optional metadata associated with the policy decision.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
