using GovernAI.Abstractions;

namespace GovernAI.Abstractions.Tests.Models;

public sealed class GovernAIPolicyDecisionTests
{
    [Fact]
    public void Create_WithRequiredDecision_ShouldInitializeCorrectly()
    {
        // Act
        var decision = new GovernAIPolicyDecision
        {
            Decision = GovernAIPolicyDecisionType.Allow
        };

        // Assert
        Assert.Equal(GovernAIPolicyDecisionType.Allow, decision.Decision);
        Assert.Equal(0, decision.RiskScore);
        Assert.Equal(GovernAIRiskLevel.None, decision.RiskLevel);
        Assert.Null(decision.Reason);
        Assert.Null(decision.RiskCategory);
        Assert.Null(decision.Metadata);
    }

    [Fact]
    public void Create_WithAllFields_ShouldInitializeAllPropertiesCorrectly()
    {
        // Arrange
        var metadata = new Dictionary<string, string> { ["source"] = "heuristic" };

        // Act
        var decision = new GovernAIPolicyDecision
        {
            Decision = GovernAIPolicyDecisionType.Deny,
            Reason = "Prompt injection detected",
            RiskScore = 95,
            RiskLevel = GovernAIRiskLevel.Critical,
            RiskCategory = "PromptInjection",
            Metadata = metadata
        };

        // Assert
        Assert.Equal(GovernAIPolicyDecisionType.Deny, decision.Decision);
        Assert.Equal("Prompt injection detected", decision.Reason);
        Assert.Equal(95, decision.RiskScore);
        Assert.Equal(GovernAIRiskLevel.Critical, decision.RiskLevel);
        Assert.Equal("PromptInjection", decision.RiskCategory);
        Assert.NotNull(decision.Metadata);
        Assert.Equal("heuristic", decision.Metadata["source"]);
    }

    [Fact]
    public void TwoDecisions_WithIdenticalValues_ShouldBeEqual()
    {
        var d1 = new GovernAIPolicyDecision { Decision = GovernAIPolicyDecisionType.Allow, RiskScore = 5 };
        var d2 = new GovernAIPolicyDecision { Decision = GovernAIPolicyDecisionType.Allow, RiskScore = 5 };

        Assert.Equal(d1, d2);
    }

    [Fact]
    public void TwoDecisions_WithDifferentDecisions_ShouldNotBeEqual()
    {
        var d1 = new GovernAIPolicyDecision { Decision = GovernAIPolicyDecisionType.Allow };
        var d2 = new GovernAIPolicyDecision { Decision = GovernAIPolicyDecisionType.Deny };

        Assert.NotEqual(d1, d2);
    }

    [Fact]
    public void Record_ShouldBeImmutable_AfterConstruction()
    {
        var original = new GovernAIPolicyDecision { Decision = GovernAIPolicyDecisionType.Allow };
        var modified = original with { Decision = GovernAIPolicyDecisionType.Review };

        Assert.Equal(GovernAIPolicyDecisionType.Allow, original.Decision);
        Assert.Equal(GovernAIPolicyDecisionType.Review, modified.Decision);
    }
}
