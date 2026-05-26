using GovernAI.Abstractions;
using GovernAI.Security;

namespace GovernAI.Security.Tests;

public sealed class RiskScoreCalculatorTests
{
    [Theory]
    [InlineData(0, GovernAIRiskLevel.None)]
    [InlineData(1, GovernAIRiskLevel.Low)]
    [InlineData(15, GovernAIRiskLevel.Low)]
    [InlineData(30, GovernAIRiskLevel.Low)]
    [InlineData(31, GovernAIRiskLevel.Medium)]
    [InlineData(45, GovernAIRiskLevel.Medium)]
    [InlineData(60, GovernAIRiskLevel.Medium)]
    [InlineData(61, GovernAIRiskLevel.High)]
    [InlineData(80, GovernAIRiskLevel.High)]
    [InlineData(85, GovernAIRiskLevel.High)]
    [InlineData(86, GovernAIRiskLevel.Critical)]
    [InlineData(100, GovernAIRiskLevel.Critical)]
    public void ScoreToLevel_MapsCorrectly(int score, GovernAIRiskLevel expected)
    {
        Assert.Equal(expected, RiskScoreCalculator.ScoreToLevel(score));
    }

    [Fact]
    public void Combine_EmptyList_ReturnsNoRisk()
    {
        var result = RiskScoreCalculator.Combine([]);
        Assert.Equal(0, result.RiskScore);
        Assert.Equal(GovernAIRiskLevel.None, result.RiskLevel);
    }

    [Fact]
    public void Combine_MultipleResults_ReturnHighestScore()
    {
        var results = new[]
        {
            new GovernAIRiskResult { RiskScore = 10, RiskLevel = GovernAIRiskLevel.Low },
            new GovernAIRiskResult { RiskScore = 80, RiskLevel = GovernAIRiskLevel.High, RiskCategory = "PromptInjection" },
            new GovernAIRiskResult { RiskScore = 20, RiskLevel = GovernAIRiskLevel.Low }
        };

        var combined = RiskScoreCalculator.Combine(results);

        Assert.Equal(80, combined.RiskScore);
        Assert.Equal(GovernAIRiskLevel.High, combined.RiskLevel);
        Assert.Equal("PromptInjection", combined.RiskCategory);
    }

    [Fact]
    public void Combine_NullResults_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => RiskScoreCalculator.Combine(null!));
    }
}
