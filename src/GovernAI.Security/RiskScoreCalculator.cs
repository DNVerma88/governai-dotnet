using GovernAI.Abstractions;

namespace GovernAI.Security;

/// <summary>
/// Calculates and combines risk scores from multiple scanner results.
/// </summary>
public static class RiskScoreCalculator
{
    /// <summary>
    /// Maps a numeric risk score to a <see cref="GovernAIRiskLevel"/>.
    /// </summary>
    /// <param name="score">The risk score in the range 0–100.</param>
    /// <returns>The corresponding <see cref="GovernAIRiskLevel"/>.</returns>
    public static GovernAIRiskLevel ScoreToLevel(int score) => score switch
    {
        0 => GovernAIRiskLevel.None,
        <= 30 => GovernAIRiskLevel.Low,
        <= 60 => GovernAIRiskLevel.Medium,
        <= 85 => GovernAIRiskLevel.High,
        _ => GovernAIRiskLevel.Critical
    };

    /// <summary>
    /// Combines multiple <see cref="GovernAIRiskResult"/> instances by taking the highest risk score.
    /// </summary>
    /// <param name="results">The scanner results to combine.</param>
    /// <returns>The combined <see cref="GovernAIRiskResult"/> with the highest risk score.</returns>
    public static GovernAIRiskResult Combine(IEnumerable<GovernAIRiskResult> results)
    {
        var list = results?.ToList() ?? throw new ArgumentNullException(nameof(results));

        if (list.Count == 0)
        {
            return new GovernAIRiskResult { RiskScore = 0, RiskLevel = GovernAIRiskLevel.None };
        }

        var best = list.MaxBy(r => r.RiskScore)!;
        var allPatterns = list
            .Where(r => r.MatchedPatterns is not null)
            .SelectMany(r => r.MatchedPatterns!)
            .Distinct()
            .ToList();

        return new GovernAIRiskResult
        {
            RiskScore = best.RiskScore,
            RiskLevel = best.RiskLevel,
            RiskCategory = best.RiskCategory,
            Reason = best.Reason,
            MatchedPatterns = allPatterns.Count > 0 ? allPatterns.AsReadOnly() : null
        };
    }
}
