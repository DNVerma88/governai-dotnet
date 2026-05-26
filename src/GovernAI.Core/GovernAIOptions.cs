namespace GovernAI.Core;

/// <summary>
/// Configuration options for the GovernAI runtime.
/// </summary>
public sealed class GovernAIOptions
{
    /// <summary>
    /// Gets or sets the application name used in all GovernAI events.
    /// </summary>
    public string ApplicationName { get; set; } = "Unknown";

    /// <summary>
    /// Gets or sets the runtime environment name (e.g., <c>Development</c>, <c>Production</c>).
    /// </summary>
    public string EnvironmentName { get; set; } = "Unknown";

    /// <summary>
    /// Gets or sets a value indicating whether prompt hashing is enabled.
    /// Default is <see langword="true"/>.
    /// </summary>
    public bool EnablePromptHashing { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether response hashing is enabled.
    /// Default is <see langword="true"/>.
    /// </summary>
    public bool EnableResponseHashing { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether raw prompt capture is allowed.
    /// Should only be enabled for local development.
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool AllowRawPromptCapture { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether raw response capture is allowed.
    /// Should only be enabled for local development.
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool AllowRawResponseCapture { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether exporter errors should propagate and fail the operation.
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool FailOnExporterError { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum number of events held in the <c>InMemoryExporter</c>.
    /// Default is <c>1000</c>.
    /// </summary>
    public int InMemoryExporterCapacity { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the file path used by the <c>FileExporter</c>.
    /// When <see langword="null"/>, the file exporter is not used.
    /// </summary>
    public string? FileExporterPath { get; set; }

    /// <summary>
    /// Gets or sets the HMAC-SHA256 key used when hashing prompts and responses.
    /// When set, hashes are keyed with HMAC-SHA256 instead of plain SHA-256,
    /// preventing rainbow-table preimage attacks on stored hashes.
    /// Must be at least 16 bytes. Recommended: 32 bytes from a cryptographically
    /// secure random source stored in a secrets manager.
    /// When <see langword="null"/>, plain SHA-256 is used.
    /// </summary>
    public byte[]? PromptHashKey { get; set; }
}
