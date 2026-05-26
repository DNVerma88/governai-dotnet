using System.Text.Json;
using System.Text.Json.Serialization;
using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// An <see cref="IGovernAIExporter"/> that writes events as JSON to the console.
/// </summary>
public sealed class ConsoleExporter : IGovernAIExporter
{
    /// <inheritdoc />
    public ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(aiEvent, GovernAIJsonContext.Default.GovernAIEvent);
        Console.WriteLine(json);
        return ValueTask.CompletedTask;
    }
}
