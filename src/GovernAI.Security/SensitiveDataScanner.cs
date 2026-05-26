using System.Text.RegularExpressions;
using GovernAI.Abstractions;

namespace GovernAI.Security;

/// <summary>
/// Scans text for sensitive data patterns and returns a <see cref="GovernAIRiskResult"/>.
/// </summary>
public sealed partial class SensitiveDataScanner
{
    [GeneratedRegex(@"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"(\+?\d{1,3}[\s\-.]?)?\(?\d{3}\)?[\s\-.]?\d{3}[\s\-.]?\d{4,6}")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"\b(?:\d{4}[\s\-]?){3}\d{4}\b")]
    private static partial Regex CreditCardRegex();

    [GeneratedRegex(@"Bearer\s+[A-Za-z0-9\-_=+/]{20,}", RegexOptions.IgnoreCase)]
    private static partial Regex BearerTokenRegex();

    [GeneratedRegex(@"eyJ[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_]+\.[A-Za-z0-9\-_.+/=]*")]
    private static partial Regex JwtRegex();

    [GeneratedRegex(@"(?i)(?:api[_\-]?key|apikey|access[_\-]?key|secret[_\-]?key)\s*[=:]\s*[A-Za-z0-9\-_.+/=]{16,}")]
    private static partial Regex ApiKeyRegex();

    [GeneratedRegex(@"(?i)(?:password|passwd|pwd)\s*[=:]\s*\S+")]
    private static partial Regex PasswordRegex();

    [GeneratedRegex(@"DefaultEndpointsProtocol=https?;AccountName=[^;]+;AccountKey=[^;]+", RegexOptions.IgnoreCase)]
    private static partial Regex AzureStorageRegex();

    [GeneratedRegex(@"(?i)(?:server|data\s+source)\s*=\s*[^;]+;[^""'\r\n]+(?:password|pwd)\s*=\s*[^;]+")]
    private static partial Regex SqlConnectionRegex();

    // Maximum number of characters scanned per call to prevent DoS (VULN-05).
    private const int MaxInputLength = 65_536;

    /// <summary>
    /// Scans the specified text for sensitive data and returns a risk result.
    /// </summary>
    /// <param name="input">The text to scan. May be <see langword="null"/> or empty.</param>
    /// <returns>A <see cref="GovernAIRiskResult"/> describing the highest risk detected.</returns>
    public GovernAIRiskResult Scan(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new GovernAIRiskResult { RiskScore = 0, RiskLevel = GovernAIRiskLevel.None };
        }

        // Cap input length to prevent DoS via regex execution on arbitrarily large strings (VULN-05).
        if (input.Length > MaxInputLength)
        {
            input = input[..MaxInputLength];
        }

        var matchedPatterns = new List<string>();
        int maxScore = 0;
        string? riskCategory = null;

        // Critical: connection strings and passwords
        if (AzureStorageRegex().IsMatch(input) || SqlConnectionRegex().IsMatch(input))
        {
            matchedPatterns.Add("ConnectionString");
            if (maxScore < 90) { maxScore = 90; riskCategory = "SensitiveData"; }
        }

        if (PasswordRegex().IsMatch(input))
        {
            matchedPatterns.Add("Password");
            if (maxScore < 90) { maxScore = 90; riskCategory = "SensitiveData"; }
        }

        // High: bearer/API key/JWT/credit card
        if (BearerTokenRegex().IsMatch(input) || JwtRegex().IsMatch(input))
        {
            matchedPatterns.Add("BearerToken/JWT");
            if (maxScore < 75) { maxScore = 75; riskCategory = "SensitiveData"; }
        }

        if (ApiKeyRegex().IsMatch(input))
        {
            matchedPatterns.Add("ApiKey");
            if (maxScore < 75) { maxScore = 75; riskCategory = "SensitiveData"; }
        }

        if (CreditCardRegex().IsMatch(input))
        {
            matchedPatterns.Add("CreditCard");
            if (maxScore < 75) { maxScore = 75; riskCategory = "SensitiveData"; }
        }

        // Low: email/phone
        if (EmailRegex().IsMatch(input))
        {
            matchedPatterns.Add("Email");
            if (maxScore < 20) { maxScore = 20; riskCategory = "SensitiveData"; }
        }

        if (PhoneRegex().IsMatch(input))
        {
            matchedPatterns.Add("PhoneNumber");
            if (maxScore < 20) { maxScore = 20; riskCategory = "SensitiveData"; }
        }

        return new GovernAIRiskResult
        {
            RiskScore = maxScore,
            RiskLevel = RiskScoreCalculator.ScoreToLevel(maxScore),
            RiskCategory = matchedPatterns.Count > 0 ? riskCategory : null,
            Reason = matchedPatterns.Count > 0
                ? $"Sensitive data detected: {string.Join(", ", matchedPatterns)}"
                : null,
            MatchedPatterns = matchedPatterns.Count > 0 ? matchedPatterns.AsReadOnly() : null
        };
    }
}
