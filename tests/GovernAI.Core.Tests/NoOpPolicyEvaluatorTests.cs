using GovernAI.Abstractions;
using GovernAI.Core;

namespace GovernAI.Core.Tests;

public sealed class NoOpPolicyEvaluatorTests
{
    [Fact]
    public async Task EvaluateAsync_AlwaysReturnsAllow()
    {
        var evaluator = NoOpPolicyEvaluator.Instance;
        var context = new GovernAIContext { OperationName = "Test" };

        var decision = await evaluator.EvaluateAsync(context);

        Assert.Equal(GovernAIPolicyDecisionType.Allow, decision.Decision);
    }

    [Fact]
    public async Task EvaluateAsync_ReturnsZeroRiskScore()
    {
        var decision = await NoOpPolicyEvaluator.Instance.EvaluateAsync(
            new GovernAIContext { OperationName = "Test" });

        Assert.Equal(0, decision.RiskScore);
    }

    [Fact]
    public async Task EvaluateAsync_ReturnsNoneRiskLevel()
    {
        var decision = await NoOpPolicyEvaluator.Instance.EvaluateAsync(
            new GovernAIContext { OperationName = "Test" });

        Assert.Equal(GovernAIRiskLevel.None, decision.RiskLevel);
    }

    [Fact]
    public async Task EvaluateAsync_ReturnsConfiguredReason()
    {
        var decision = await NoOpPolicyEvaluator.Instance.EvaluateAsync(
            new GovernAIContext { OperationName = "Test" });

        Assert.Equal("No policy evaluator configured", decision.Reason);
    }
}
