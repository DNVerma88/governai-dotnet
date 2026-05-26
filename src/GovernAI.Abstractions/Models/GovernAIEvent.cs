namespace GovernAI.Abstractions;

/// <summary>
/// Represents a GovernAI governance and audit event produced during a tracked AI operation.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="GovernAIEvent"/> is the primary carrier of governance data exported by the SDK.
/// It is designed to be provider-neutral, cloud-neutral, tenant-aware, and privacy-safe.
/// </para>
/// <para>
/// Raw prompt and response text are not stored by default. SHA-256 hashes are used instead.
/// </para>
/// <para>
/// The schema is collector-ready and future-compatible with OpenTelemetry GenAI conventions.
/// </para>
/// </remarks>
public sealed record GovernAIEvent
{
    /// <summary>
    /// Gets the unique identifier for this event.
    /// </summary>
    /// <remarks>Recommended format is a GUID without hyphens.</remarks>
    public required string EventId { get; init; }

    /// <summary>
    /// Gets the distributed trace identifier for correlation with application logs and telemetry.
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// Gets the request or operation-level correlation identifier.
    /// </summary>
    /// <remarks>
    /// In ASP.NET Core, this may come from the <c>X-Correlation-Id</c> HTTP header.
    /// If missing, GovernAI generates one automatically.
    /// </remarks>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the name of the application using GovernAI.
    /// </summary>
    public required string ApplicationName { get; init; }

    /// <summary>
    /// Gets the name of the runtime environment (e.g., <c>Development</c>, <c>Staging</c>, <c>Production</c>).
    /// </summary>
    public string? EnvironmentName { get; init; }

    /// <summary>
    /// Gets the tenant identifier for multi-tenant applications.
    /// </summary>
    /// <remarks>Can be <see langword="null"/> for single-tenant applications.</remarks>
    public string? TenantId { get; init; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    /// <remarks>
    /// Should be resolved from claims where possible.
    /// Can be <see langword="null"/> for system or background operations.
    /// Must not store full user profile data.
    /// </remarks>
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
    /// Gets the name of the AI model provider (e.g., <c>AzureOpenAI</c>, <c>OpenAI</c>, <c>Anthropic</c>).
    /// </summary>
    /// <remarks>GovernAI does not depend on any specific provider SDK.</remarks>
    public string? ModelProvider { get; init; }

    /// <summary>
    /// Gets the name of the AI model (e.g., <c>gpt-4.1</c>, <c>claude-sonnet</c>, <c>gemini-pro</c>).
    /// </summary>
    public string? ModelName { get; init; }

    /// <summary>
    /// Gets the SHA-256 hash of the prompt.
    /// </summary>
    /// <remarks>Raw prompts are not stored by default. <see langword="null"/> when no prompt is provided.</remarks>
    public string? PromptHash { get; init; }

    /// <summary>
    /// Gets the SHA-256 hash of the response.
    /// </summary>
    /// <remarks>Raw responses are not stored by default. <see langword="null"/> when no response is provided.</remarks>
    public string? ResponseHash { get; init; }

    /// <summary>
    /// Gets the raw prompt text.
    /// </summary>
    /// <remarks>
    /// Only populated when <c>AllowRawPromptCapture</c> is <see langword="true"/> in <c>GovernAIOptions</c>.
    /// Should only be enabled in development environments.
    /// </remarks>
    public string? RawPrompt { get; init; }

    /// <summary>
    /// Gets the raw response text.
    /// </summary>
    /// <remarks>
    /// Only populated when <c>AllowRawResponseCapture</c> is <see langword="true"/> in <c>GovernAIOptions</c>.
    /// Should only be enabled in development environments.
    /// </remarks>
    public string? RawResponse { get; init; }

    /// <summary>
    /// Gets the input token count if available.
    /// </summary>
    /// <remarks>Optional. Applications may provide this value. The SDK does not calculate tokens in MVP.</remarks>
    public int? InputTokens { get; init; }

    /// <summary>
    /// Gets the output token count if available.
    /// </summary>
    /// <remarks>Optional. Applications may provide this value. The SDK does not calculate tokens in MVP.</remarks>
    public int? OutputTokens { get; init; }

    /// <summary>
    /// Gets the total token count.
    /// </summary>
    /// <remarks>
    /// Typically computed as <c>InputTokens + OutputTokens</c> when both are available.
    /// Applications may provide this value directly.
    /// </remarks>
    public int? TotalTokens { get; init; }

    /// <summary>
    /// Gets the numeric risk score in the range 0–100.
    /// </summary>
    public int RiskScore { get; init; }

    /// <summary>
    /// Gets the risk classification level.
    /// </summary>
    public GovernAIRiskLevel RiskLevel { get; init; }

    /// <summary>
    /// Gets the human-readable risk category (e.g., <c>PromptInjection</c>, <c>SensitiveData</c>, <c>Unknown</c>).
    /// </summary>
    public string? RiskCategory { get; init; }

    /// <summary>
    /// Gets the policy decision for this operation.
    /// </summary>
    public GovernAIPolicyDecisionType PolicyDecision { get; init; }

    /// <summary>
    /// Gets the human-readable reason for the policy decision.
    /// </summary>
    public string? PolicyReason { get; init; }

    /// <summary>
    /// Gets the operation duration in milliseconds.
    /// </summary>
    /// <remarks>Measured from before policy evaluation through event creation.</remarks>
    public long DurationMs { get; init; }

    /// <summary>
    /// Gets a value indicating whether the tracked AI operation completed successfully.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the optional error code when the operation fails.
    /// </summary>
    /// <remarks><see langword="null"/> when the operation succeeds.</remarks>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the optional sanitized error message.
    /// </summary>
    /// <remarks>Must not contain secrets, tokens, or raw prompt/response content.</remarks>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when this event was created.
    /// </summary>
    public required DateTimeOffset TimestampUtc { get; init; }

    /// <summary>
    /// Gets the string-based metadata dictionary for extensibility.
    /// </summary>
    /// <remarks>
    /// Keys and values must be strings for cross-language compatibility.
    /// Must not store secrets or raw prompts.
    /// </remarks>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
