namespace GovernAI.Abstractions;

/// <summary>
/// Defines the contract for redacting sensitive information from text.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to provide custom redaction logic. Implementations should
/// redact sensitive values such as email addresses, phone numbers, API keys, bearer tokens,
/// JWT tokens, passwords, and connection strings.
/// </para>
/// <para>
/// Implementations must be thread-safe.
/// Redaction must not store or log the original sensitive values.
/// </para>
/// </remarks>
public interface IGovernAIRedactor
{
    /// <summary>
    /// Redacts sensitive information from the specified text.
    /// </summary>
    /// <param name="input">The text to scan and redact. May be <see langword="null"/> or empty.</param>
    /// <returns>
    /// The redacted text with sensitive information replaced by redaction markers,
    /// or <see cref="string.Empty"/> when <paramref name="input"/> is <see langword="null"/> or empty.
    /// </returns>
    string Redact(string? input);
}
