namespace GovernAI.Abstractions;

/// <summary>
/// Represents the result of a risk scan performed by a security scanner.
/// </summary>
public sealed record GovernAIRiskResult
{
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
    /// Gets the human-readable reason describing the detected risk.
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// Gets the collection of patterns that were matched during the risk scan.
    /// </summary>
    public IReadOnlyList<string>? MatchedPatterns { get; init; }
}
