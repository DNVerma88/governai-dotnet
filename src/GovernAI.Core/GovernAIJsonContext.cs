using System.Text.Json;
using System.Text.Json.Serialization;
using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// AOT-compatible JSON serializer context for GovernAI types.
/// </summary>
[JsonSerializable(typeof(GovernAIEvent))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, string>))]
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public sealed partial class GovernAIJsonContext : JsonSerializerContext
{
}
