using GovernAI.Abstractions;
using GovernAI.Core;

namespace GovernAI.Core.Tests;

public sealed class InMemoryExporterTests
{
    [Fact]
    public async Task ExportAsync_AddsEventToCollection()
    {
        var exporter = new InMemoryExporter();
        var @event = MakeEvent();

        await exporter.ExportAsync(@event);

        Assert.Single(exporter.Events);
    }

    [Fact]
    public async Task ExportAsync_MultipleEvents_AllStoredInOrder()
    {
        var exporter = new InMemoryExporter();

        for (var i = 0; i < 5; i++)
        {
            await exporter.ExportAsync(MakeEvent($"evt-{i}"));
        }

        Assert.Equal(5, exporter.Events.Count);
        Assert.Equal("evt-0", exporter.Events[0].EventId);
        Assert.Equal("evt-4", exporter.Events[4].EventId);
    }

    [Fact]
    public async Task ExportAsync_OverCapacity_OldestEventsEvicted()
    {
        var exporter = new InMemoryExporter(capacity: 3);

        for (var i = 0; i < 5; i++)
        {
            await exporter.ExportAsync(MakeEvent($"evt-{i}"));
        }

        Assert.Equal(3, exporter.Events.Count);
    }

    [Fact]
    public void Constructor_ZeroCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new InMemoryExporter(0));
    }

    [Fact]
    public void Constructor_NegativeCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new InMemoryExporter(-1));
    }

    [Fact]
    public async Task Clear_RemovesAllEvents()
    {
        var exporter = new InMemoryExporter();
        await exporter.ExportAsync(MakeEvent());
        await exporter.ExportAsync(MakeEvent());

        exporter.Clear();

        Assert.Empty(exporter.Events);
    }

    private static GovernAIEvent MakeEvent(string eventId = "test-event") =>
        new GovernAIEvent
        {
            EventId = eventId,
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };
}

public sealed class FileExporterTests
{
    [Fact]
    public void FileExporter_ImplementsIDisposable()
    {
        // IDisposable is required to release the internal SemaphoreSlim.
        Assert.True(typeof(IDisposable).IsAssignableFrom(typeof(FileExporter)));
    }

    [Fact]
    public void FileExporter_Dispose_DoesNotThrow()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jsonl");
        using var exporter = new FileExporter(path);
        // Disposing should not throw even when no writes have occurred.
    }

    [Fact]
    public async Task FileExporter_AfterDispose_ExportAsyncThrowsObjectDisposedException()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jsonl");
        var exporter = new FileExporter(path);
        exporter.Dispose();

        var @event = new GovernAIEvent
        {
            EventId = "e1",
            ApplicationName = "App",
            OperationName = "Op",
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };

        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            exporter.ExportAsync(@event).AsTask());
    }

    [Fact]
    public void FileExporter_RelativePath_Throws()
    {
        Assert.Throws<ArgumentException>(() => new FileExporter("relative/path/file.jsonl"));
    }
}
