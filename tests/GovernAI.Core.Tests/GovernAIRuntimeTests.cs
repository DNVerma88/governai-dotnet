using GovernAI.Abstractions;
using GovernAI.Core;

namespace GovernAI.Core.Tests;

public sealed class GovernAIRuntimeTests
{
    private static readonly GovernAIOptions DefaultOptions = new()
    {
        ApplicationName = "TestApp",
        EnvironmentName = "Test"
    };

    private static GovernAIRuntime BuildRuntime(
        IGovernAIPolicyEvaluator? policy = null,
        IGovernAIExporter? exporter = null)
    {
        var tracker = new GovernAITracker(
            DefaultOptions,
            SystemClock.Instance,
            policy ?? NoOpPolicyEvaluator.Instance,
            exporter ?? NoOpExporter.Instance);

        return new GovernAIRuntime(tracker);
    }

    [Fact]
    public async Task TrackAsync_SuccessfulOperation_ReturnsResult()
    {
        var runtime = BuildRuntime();
        var context = new GovernAIContext { OperationName = "Test", Prompt = "Hello" };

        var result = await runtime.TrackAsync(context, _ => Task.FromResult("response"));

        Assert.Equal("response", result);
    }

    [Fact]
    public async Task TrackAsync_DelegatesToTracker_ExportsEvent()
    {
        var exporter = new InMemoryExporter();
        var runtime = BuildRuntime(exporter: exporter);
        var context = new GovernAIContext { OperationName = "SomeOp" };

        await runtime.TrackAsync(context, _ => Task.FromResult("output"));

        Assert.Single(exporter.Events);
        Assert.True(exporter.Events[0].Success);
        Assert.Equal("SomeOp", exporter.Events[0].OperationName);
    }

    [Fact]
    public async Task TrackAsync_PolicyDeny_ThrowsGovernAIDeniedException()
    {
        var runtime = BuildRuntime(policy: new AlwaysDenyEvaluator());
        var context = new GovernAIContext { OperationName = "Test" };

        await Assert.ThrowsAsync<GovernAIDeniedException>(() =>
            runtime.TrackAsync(context, _ => Task.FromResult("r")));
    }

    [Fact]
    public async Task TrackAsync_PolicyDeny_DoesNotExecuteOperation()
    {
        var runtime = BuildRuntime(policy: new AlwaysDenyEvaluator());
        var context = new GovernAIContext { OperationName = "Test" };
        var executed = false;

        try
        {
            await runtime.TrackAsync(context, _ =>
            {
                executed = true;
                return Task.FromResult("r");
            });
        }
        catch (GovernAIDeniedException) { }

        Assert.False(executed);
    }

    [Fact]
    public async Task TrackAsync_VoidOverload_ExecutesOperation()
    {
        var exporter = new InMemoryExporter();
        var runtime = BuildRuntime(exporter: exporter);
        var executed = false;
        var context = new GovernAIContext { OperationName = "Test" };

        await runtime.TrackAsync(context, _ =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        Assert.True(executed);
        Assert.Single(exporter.Events);
    }

    [Fact]
    public void Constructor_NullTracker_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GovernAIRuntime(null!));
    }

    private sealed class AlwaysDenyEvaluator : IGovernAIPolicyEvaluator
    {
        public ValueTask<GovernAIPolicyDecision> EvaluateAsync(
            GovernAIContext context, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new GovernAIPolicyDecision
            {
                Decision = GovernAIPolicyDecisionType.Deny,
                Reason = "Test denial",
                RiskScore = 100,
                RiskLevel = GovernAIRiskLevel.Critical
            });
    }
}
