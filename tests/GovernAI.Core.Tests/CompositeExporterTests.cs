using GovernAI.Abstractions;
using GovernAI.Core;

namespace GovernAI.Core.Tests;

public sealed class CompositeExporterTests
{
    [Fact]
    public async Task ExportAsync_CallsAllExporters()
    {
        var exp1 = new InMemoryExporter();
        var exp2 = new InMemoryExporter();
        var composite = new CompositeExporter([exp1, exp2]);
        var @event = MakeEvent();

        await composite.ExportAsync(@event);

        Assert.Single(exp1.Events);
        Assert.Single(exp2.Events);
    }

    [Fact]
    public async Task ExportAsync_WhenOneExporterFails_ContinuesToNext_ByDefault()
    {
        var failingExporter = new FailingExporter();
        var goodExporter = new InMemoryExporter();
        var composite = new CompositeExporter([failingExporter, goodExporter], failOnExporterError: false);

        await composite.ExportAsync(MakeEvent()); // should not throw

        Assert.Single(goodExporter.Events);
    }

    [Fact]
    public async Task ExportAsync_WhenOneExporterFails_ThrowsWhenConfigured()
    {
        var failingExporter = new FailingExporter();
        var goodExporter = new InMemoryExporter();
        var composite = new CompositeExporter([failingExporter, goodExporter], failOnExporterError: true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            composite.ExportAsync(MakeEvent()).AsTask());
    }

    [Fact]
    public void Constructor_NullExporters_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CompositeExporter(null!));
    }

    [Fact]
    public async Task ExportAsync_WhenChildFails_LogsErrorAndContinues()
    {
        // Use a capturing logger to verify the failure is recorded.
        var logger = new CapturingLogger();
        var failingExporter = new FailingExporter();
        var goodExporter = new InMemoryExporter();
        var composite = new CompositeExporter(
            [failingExporter, goodExporter],
            failOnExporterError: false,
            logger);

        await composite.ExportAsync(MakeEvent());

        Assert.Single(goodExporter.Events);
        Assert.True(logger.HasLoggedError,
            "Expected CompositeExporter to log an error when a child exporter fails.");
    }

    private static GovernAIEvent MakeEvent() =>
        new GovernAIEvent
        {
            EventId = "test",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };

    private sealed class FailingExporter : IGovernAIExporter
    {
        public ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Export failed");
    }

    private sealed class CapturingLogger : Microsoft.Extensions.Logging.ILogger<CompositeExporter>
    {
        public bool HasLoggedError { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;

        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.Error)
            {
                HasLoggedError = true;
            }
        }
    }
}
