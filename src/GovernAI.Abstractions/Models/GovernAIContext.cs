namespace GovernAI.Abstractions;

/// <summary>
/// Represents the context for a GovernAI-tracked AI operation.
/// </summary>
/// <remarks>
/// <see cref="GovernAIContext"/> carries the inputs and metadata that describe an AI operation
/// before it is executed and tracked. It is passed to policy evaluators and used to construct
/// the resulting <see cref="GovernAIEvent"/>.
/// </remarks>
public sealed record GovernAIContext
{
    /// <summary>
    /// Gets the distributed trace identifier for correlation with application telemetry.
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// Gets the request or operation-level correlation identifier.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the name of the application. When set, overrides the application name from options.
    /// </summary>
    public string? ApplicationName { get; init; }

    /// <summary>
    /// Gets the runtime environment name. When set, overrides the environment name from options.
    /// </summary>
    public string? EnvironmentName { get; init; }

    /// <summary>
    /// Gets the tenant identifier. When set, overrides the tenant resolved from the current context.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Gets the user identifier. When set, overrides the user resolved from the current context.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets the logical AI agent or workflow name.
    /// </summary>
    public string? AgentName { get; init; }

    /// <summary>
    /// Gets the name of the AI operation being tracked.
    /// </summary>
    public required string OperationName { get; init; }

    /// <summary>
    /// Gets the AI model provider name (e.g., <c>AzureOpenAI</c>, <c>OpenAI</c>, <c>Anthropic</c>).
    /// </summary>
    public string? ModelProvider { get; init; }

    /// <summary>
    /// Gets the AI model name (e.g., <c>gpt-4.1</c>, <c>claude-sonnet</c>).
    /// </summary>
    public string? ModelName { get; init; }

    /// <summary>
    /// Gets the raw prompt text for the AI operation.
    /// </summary>
    /// <remarks>
    /// Used for hashing and passed to the AI operation delegate.
    /// Not stored in the resulting <see cref="GovernAIEvent"/> by default.
    /// </remarks>
    public string? Prompt { get; init; }

    /// <summary>
    /// Gets the raw response text. Optional; used for post-facto tracking scenarios.
    /// </summary>
    /// <remarks>Not stored in the resulting <see cref="GovernAIEvent"/> by default.</remarks>
    public string? Response { get; init; }

    /// <summary>
    /// Gets the input token count if known ahead of execution.
    /// </summary>
    public int? InputTokens { get; init; }

    /// <summary>
    /// Gets the output token count if known ahead of execution.
    /// </summary>
    public int? OutputTokens { get; init; }

    /// <summary>
    /// Gets the string-based metadata dictionary for extensibility.
    /// </summary>
    /// <remarks>Keys and values must be strings. Must not store secrets or raw prompts.</remarks>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
