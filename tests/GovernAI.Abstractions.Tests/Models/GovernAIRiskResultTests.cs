using GovernAI.Abstractions;

namespace GovernAI.Abstractions.Tests.Models;

public sealed class GovernAIRiskResultTests
{
    [Fact]
    public void Create_WithDefaults_ShouldInitializeCorrectly()
    {
        // Act
        var result = new GovernAIRiskResult();

        // Assert
        Assert.Equal(0, result.RiskScore);
        Assert.Equal(GovernAIRiskLevel.None, result.RiskLevel);
        Assert.Null(result.RiskCategory);
        Assert.Null(result.Reason);
        Assert.Null(result.MatchedPatterns);
    }

    [Fact]
    public void Create_WithAllFields_ShouldInitializeAllPropertiesCorrectly()
    {
        // Arrange
        var patterns = new List<string> { "ignore previous instructions", "system prompt" };

        // Act
        var result = new GovernAIRiskResult
        {
            RiskScore = 85,
            RiskLevel = GovernAIRiskLevel.High,
            RiskCategory = "PromptInjection",
            Reason = "Prompt injection pattern detected",
            MatchedPatterns = patterns
        };

        // Assert
        Assert.Equal(85, result.RiskScore);
        Assert.Equal(GovernAIRiskLevel.High, result.RiskLevel);
        Assert.Equal("PromptInjection", result.RiskCategory);
        Assert.Equal("Prompt injection pattern detected", result.Reason);
        Assert.NotNull(result.MatchedPatterns);
        Assert.Equal(2, result.MatchedPatterns.Count);
        Assert.Contains("ignore previous instructions", result.MatchedPatterns);
    }

    [Fact]
    public void TwoResults_WithIdenticalValues_ShouldBeEqual()
    {
        var r1 = new GovernAIRiskResult { RiskScore = 10, RiskLevel = GovernAIRiskLevel.Low };
        var r2 = new GovernAIRiskResult { RiskScore = 10, RiskLevel = GovernAIRiskLevel.Low };

        Assert.Equal(r1, r2);
    }

    [Fact]
    public void Record_ShouldBeImmutable_AfterConstruction()
    {
        var original = new GovernAIRiskResult { RiskScore = 10 };
        var modified = original with { RiskScore = 50 };

        Assert.Equal(10, original.RiskScore);
        Assert.Equal(50, modified.RiskScore);
    }
}
