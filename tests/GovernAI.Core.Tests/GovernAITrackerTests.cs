using GovernAI.Abstractions;
using GovernAI.Core;

namespace GovernAI.Core.Tests;

public sealed class GovernAITrackerTests
{
    private static GovernAIOptions DefaultOptions => new()
    {
        ApplicationName = "TestApp",
        EnvironmentName = "Test"
    };

    private static GovernAITracker BuildTracker(
        GovernAIOptions? options = null,
        IGovernAIPolicyEvaluator? policy = null,
        IGovernAIExporter? exporter = null,
        IGovernAIClock? clock = null)
    {
        return new GovernAITracker(
            options ?? DefaultOptions,
            clock ?? SystemClock.Instance,
            policy ?? NoOpPolicyEvaluator.Instance,
            exporter ?? NoOpExporter.Instance);
    }

    [Fact]
    public async Task TrackAsync_SuccessfulOperation_ExportsEvent()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "GenerateSummary", Prompt = "Test prompt" };

        var result = await tracker.TrackAsync(context, _ => Task.FromResult("AI response"));

        Assert.Equal("AI response", result);
        Assert.Single(exporter.Events);
        Assert.True(exporter.Events[0].Success);
        Assert.Equal("GenerateSummary", exporter.Events[0].OperationName);
    }

    [Fact]
    public async Task TrackAsync_SuccessfulOperation_HashesPrompt()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test", Prompt = "hello" };

        await tracker.TrackAsync(context, _ => Task.FromResult("response"));

        var @event = exporter.Events[0];
        Assert.NotNull(@event.PromptHash);
        Assert.Equal(64, @event.PromptHash!.Length); // SHA-256 hex
    }

    [Fact]
    public async Task TrackAsync_SuccessfulOperation_HashesStringResponse()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };

        await tracker.TrackAsync(context, _ => Task.FromResult("my response text"));

        Assert.NotNull(exporter.Events[0].ResponseHash);
        Assert.Equal(64, exporter.Events[0].ResponseHash!.Length);
    }

    [Fact]
    public async Task TrackAsync_SuccessfulOperation_CapturesDuration()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };

        await tracker.TrackAsync(context, async ct =>
        {
            await Task.Delay(50, ct);
            return "response";
        });

        Assert.True(exporter.Events[0].DurationMs >= 0);
    }

    [Fact]
    public async Task TrackAsync_FailedOperation_ExportsFailureEvent()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            tracker.TrackAsync<string>(context, _ => throw new InvalidOperationException("AI failed")));

        Assert.Single(exporter.Events);
        Assert.False(exporter.Events[0].Success);
        Assert.Equal("InvalidOperationException", exporter.Events[0].ErrorCode);
        Assert.Equal("AI failed", exporter.Events[0].ErrorMessage);
    }

    [Fact]
    public async Task TrackAsync_FailedOperation_RethrowsException()
    {
        var tracker = BuildTracker();
        var context = new GovernAIContext { OperationName = "Test" };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            tracker.TrackAsync<string>(context, _ => throw new InvalidOperationException("fail")));
    }

    [Fact]
    public async Task TrackAsync_PolicyDeny_ThrowsGovernAIDeniedException()
    {
        var denyEvaluator = new AlwaysDenyEvaluator();
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(policy: denyEvaluator, exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };

        var ex = await Assert.ThrowsAsync<GovernAIDeniedException>(() =>
            tracker.TrackAsync<string>(context, _ => Task.FromResult("result")));

        Assert.Equal(GovernAIPolicyDecisionType.Deny, ex.PolicyDecision.Decision);
    }

    [Fact]
    public async Task TrackAsync_PolicyDeny_DoesNotExecuteOperation()
    {
        var denyEvaluator = new AlwaysDenyEvaluator();
        var tracker = BuildTracker(policy: denyEvaluator);
        var context = new GovernAIContext { OperationName = "Test" };
        var executed = false;

        try
        {
            await tracker.TrackAsync(context, _ =>
            {
                executed = true;
                return Task.FromResult("result");
            });
        }
        catch (GovernAIDeniedException) { }

        Assert.False(executed);
    }

    [Fact]
    public async Task TrackAsync_PolicyDeny_ExportsFailureEvent()
    {
        var denyEvaluator = new AlwaysDenyEvaluator();
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(policy: denyEvaluator, exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };

        try { await tracker.TrackAsync<string>(context, _ => Task.FromResult("r")); }
        catch (GovernAIDeniedException) { }

        Assert.Single(exporter.Events);
        Assert.False(exporter.Events[0].Success);
        Assert.Equal("POLICY_DENIED", exporter.Events[0].ErrorCode);
    }

    [Fact]
    public async Task TrackAsync_TotalTokens_CalculatedWhenBothPresent()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext
        {
            OperationName = "Test",
            InputTokens = 100,
            OutputTokens = 50
        };

        await tracker.TrackAsync(context, _ => Task.FromResult("response"));

        Assert.Equal(150, exporter.Events[0].TotalTokens);
    }

    [Fact]
    public async Task TrackAsync_TotalTokens_NullWhenOnlyInputPresent()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test", InputTokens = 100 };

        await tracker.TrackAsync(context, _ => Task.FromResult("response"));

        Assert.Null(exporter.Events[0].TotalTokens);
    }

    [Fact]
    public async Task TrackAsync_HashingDisabled_NullHashes()
    {
        var exporter = new InMemoryExporter();
        var options = new GovernAIOptions
        {
            EnablePromptHashing = false,
            EnableResponseHashing = false
        };
        var tracker = BuildTracker(options: options, exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test", Prompt = "hello" };

        await tracker.TrackAsync(context, _ => Task.FromResult("response"));

        Assert.Null(exporter.Events[0].PromptHash);
        Assert.Null(exporter.Events[0].ResponseHash);
    }

    [Fact]
    public async Task TrackAsync_ExporterFails_DoesNotThrowByDefault()
    {
        var failExporter = new FailingExporter();
        var tracker = BuildTracker(exporter: failExporter, options: new GovernAIOptions { FailOnExporterError = false });
        var context = new GovernAIContext { OperationName = "Test" };

        // Should not throw despite exporter failure
        var result = await tracker.TrackAsync(context, _ => Task.FromResult("ok"));
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task TrackAsync_ExporterFails_ThrowsWhenConfigured()
    {
        var failExporter = new FailingExporter();
        var tracker = BuildTracker(exporter: failExporter, options: new GovernAIOptions { FailOnExporterError = true });
        var context = new GovernAIContext { OperationName = "Test" };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            tracker.TrackAsync(context, _ => Task.FromResult("ok")));
    }

    [Fact]
    public async Task TrackAsync_EventHasExpectedApplicationName()
    {
        var exporter = new InMemoryExporter();
        var options = new GovernAIOptions { ApplicationName = "MyApp", EnvironmentName = "Prod" };
        var tracker = BuildTracker(options: options, exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };

        await tracker.TrackAsync(context, _ => Task.FromResult("r"));

        Assert.Equal("MyApp", exporter.Events[0].ApplicationName);
        Assert.Equal("Prod", exporter.Events[0].EnvironmentName);
    }

    [Fact]
    public async Task TrackAsync_AllowRawPromptCapture_StoresRawPrompt()
    {
        var exporter = new InMemoryExporter();
        var options = new GovernAIOptions
        {
            ApplicationName = "TestApp",
            AllowRawPromptCapture = true
        };
        var tracker = new GovernAITracker(options, SystemClock.Instance, NoOpPolicyEvaluator.Instance, exporter);
        var context = new GovernAIContext { OperationName = "Test", Prompt = "raw prompt text" };

        await tracker.TrackAsync(context, _ => Task.FromResult("response"));

        Assert.Equal("raw prompt text", exporter.Events[0].RawPrompt);
    }

    [Fact]
    public async Task TrackAsync_AllowRawResponseCapture_StoresRawResponse()
    {
        var exporter = new InMemoryExporter();
        var options = new GovernAIOptions
        {
            ApplicationName = "TestApp",
            AllowRawResponseCapture = true
        };
        var tracker = new GovernAITracker(options, SystemClock.Instance, NoOpPolicyEvaluator.Instance, exporter);
        var context = new GovernAIContext { OperationName = "Test" };

        await tracker.TrackAsync(context, _ => Task.FromResult("raw response text"));

        Assert.Equal("raw response text", exporter.Events[0].RawResponse);
    }

    [Fact]
    public async Task TrackAsync_RawCaptureDisabled_NullRawFields()
    {
        var exporter = new InMemoryExporter();
        var options = new GovernAIOptions
        {
            ApplicationName = "TestApp",
            AllowRawPromptCapture = false,
            AllowRawResponseCapture = false
        };
        var tracker = new GovernAITracker(options, SystemClock.Instance, NoOpPolicyEvaluator.Instance, exporter);
        var context = new GovernAIContext { OperationName = "Test", Prompt = "secret prompt" };

        await tracker.TrackAsync(context, _ => Task.FromResult("secret response"));

        Assert.Null(exporter.Events[0].RawPrompt);
        Assert.Null(exporter.Events[0].RawResponse);
    }

    [Fact]
    public async Task TrackAsync_ContextWithMetadata_MetadataForwardedToEvent()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var metadata = new Dictionary<string, string> { ["key1"] = "value1", ["key2"] = "value2" };
        var context = new GovernAIContext
        {
            OperationName = "Test",
            Metadata = metadata
        };

        await tracker.TrackAsync(context, _ => Task.FromResult("response"));

        var evt = exporter.Events[0];
        Assert.NotNull(evt.Metadata);
        Assert.Equal("value1", evt.Metadata!["key1"]);
        Assert.Equal("value2", evt.Metadata!["key2"]);
    }

    [Fact]
    public async Task TrackAsync_ContextWithNullMetadata_MetadataIsNullInEvent()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test", Metadata = null };

        await tracker.TrackAsync(context, _ => Task.FromResult("response"));

        Assert.Null(exporter.Events[0].Metadata);
    }

    [Fact]
    public async Task TrackAsync_FailedOperation_ErrorMessageStripsControlCharacters()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };
        // Embed a newline (log injection attempt) and a tab in the exception message.
        var maliciousMessage = "Error line1\nINFO: injected log\ttab";

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            tracker.TrackAsync<string>(context, _ => throw new InvalidOperationException(maliciousMessage)));

        var errorMessage = exporter.Events[0].ErrorMessage;
        Assert.NotNull(errorMessage);
        Assert.DoesNotContain("\n", errorMessage);
        Assert.DoesNotContain("\t", errorMessage);
    }

    [Fact]
    public async Task TrackAsync_FailedOperation_ErrorMessageTruncatedAt500Chars()
    {
        var exporter = new InMemoryExporter();
        var tracker = BuildTracker(exporter: exporter);
        var context = new GovernAIContext { OperationName = "Test" };
        var longMessage = new string('x', 1000);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            tracker.TrackAsync<string>(context, _ => throw new InvalidOperationException(longMessage)));

        Assert.True(exporter.Events[0].ErrorMessage!.Length <= 500);
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

    private sealed class FailingExporter : IGovernAIExporter
    {
        public ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Export failed intentionally");
    }
}
