using System.Text.RegularExpressions;
using GovernAI.Abstractions;

namespace GovernAI.Security;

/// <summary>
/// A heuristic-based PII and sensitive data redactor.
/// </summary>
/// <remarks>
/// Detects and redacts email addresses, phone numbers, credit card-like numbers,
/// API keys, bearer tokens, JWT-like tokens, connection strings, and password-like pairs.
/// This implementation is heuristic-based and not a substitute for a full data-loss-prevention system.
/// </remarks>
public sealed partial class BasicPiiRedactor : IGovernAIRedactor
{
    // Email addresses: local@domain.tld
    [GeneratedRegex(@"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    // Phone numbers: various formats including +1 (555) 555-5555, 555-555-5555, +44 20 7946 0958
    [GeneratedRegex(@"(\+?\d{1,3}[\s\-.]?)?\(?\d{3}\)?[\s\-.]?\d{3}[\s\-.]?\d{4,6}")]
    private static partial Regex PhoneRegex();

    // Credit card-like: 16-digit numbers with optional separators
    [GeneratedRegex(@"\b(?:\d{4}[\s\-]?){3}\d{4}\b")]
    private static partial Regex CreditCardRegex();

    // Bearer tokens
    [GeneratedRegex(@"Bearer\s+[A-Za-z0-9\-_=+/]{20,}", RegexOptions.IgnoreCase)]
    private static partial Regex BearerTokenRegex();

    // JWT-like: three base64url segments separated by dots
    [GeneratedRegex(@"eyJ[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_.+/=]*")]
    private static partial Regex JwtRegex();

    // API key patterns: key=<value> or apikey: <value> or api_key=<value>
    [GeneratedRegex(@"(?i)(?:api[_\-]?key|apikey|access[_\-]?key|secret[_\-]?key)\s*[=:]\s*[A-Za-z0-9\-_.+/=]{16,}")]
    private static partial Regex ApiKeyRegex();

    // SQL Server / Azure SQL connection strings
    [GeneratedRegex(@"(?i)(?:server|data\s+source)\s*=\s*[^;]+;[^""'\r\n]+(?:password|pwd)\s*=\s*[^;]+", RegexOptions.IgnoreCase)]
    private static partial Regex SqlConnectionStringRegex();

    // Azure Storage connection strings
    [GeneratedRegex(@"DefaultEndpointsProtocol=https?;AccountName=[^;]+;AccountKey=[^;]+(?:;[^\s""']*)?", RegexOptions.IgnoreCase)]
    private static partial Regex AzureStorageConnectionStringRegex();

    // Password-like key/value pairs: password=xxx, pwd=xxx, passwd=xxx
    [GeneratedRegex(@"(?i)(?:password|passwd|pwd)\s*[=:]\s*\S+")]
    private static partial Regex PasswordRegex();

    // Maximum number of characters processed per call to prevent DoS (VULN-05).
    private const int MaxInputLength = 65_536;

    /// <inheritdoc />
    public string Redact(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        // Cap input length to prevent DoS via regex execution on arbitrarily large strings (VULN-05).
        if (input.Length > MaxInputLength)
        {
            input = input[..MaxInputLength];
        }

        var result = input;

        // Apply redactions in order of specificity (most specific first)
        result = JwtRegex().Replace(result, "[REDACTED_TOKEN]");
        result = BearerTokenRegex().Replace(result, "Bearer [REDACTED_TOKEN]");
        result = AzureStorageConnectionStringRegex().Replace(result, "[REDACTED_CONNECTION_STRING]");
        result = SqlConnectionStringRegex().Replace(result, "[REDACTED_CONNECTION_STRING]");
        result = ApiKeyRegex().Replace(result, "[REDACTED_API_KEY]");
        result = PasswordRegex().Replace(result, "[REDACTED_SECRET]");
        result = CreditCardRegex().Replace(result, "[REDACTED_CARD]");
        result = EmailRegex().Replace(result, "[REDACTED_EMAIL]");
        result = PhoneRegex().Replace(result, "[REDACTED_PHONE]");

        return result;
    }
}
