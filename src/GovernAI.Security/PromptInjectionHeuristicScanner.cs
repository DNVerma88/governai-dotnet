using System.Text.RegularExpressions;
using GovernAI.Abstractions;

namespace GovernAI.Security;

/// <summary>
/// Scans text for prompt injection-like heuristic patterns and returns a <see cref="GovernAIRiskResult"/>.
/// </summary>
/// <remarks>
/// This scanner is heuristic-based and intended for governance awareness only.
/// It is not a guaranteed defense against all prompt injection attacks.
/// </remarks>
public sealed partial class PromptInjectionHeuristicScanner
{
    // System/hidden prompt extraction
    [GeneratedRegex(@"(?i)(reveal|show|print|display|output|expose)\s+(the\s+)?(system|hidden|original|initial|base)\s+prompt", RegexOptions.IgnoreCase)]
    private static partial Regex SystemPromptExtractionRegex();

    // Secret extraction
    [GeneratedRegex(@"(?i)(print|show|reveal|expose|leak|exfiltrate)\s+(?:(?:all|the|any|your|my|those|these)\s+)*(secrets?|credentials?|api[\s_-]?keys?|passwords?|tokens?)", RegexOptions.IgnoreCase)]
    private static partial Regex SecretExtractionRegex();

    // Bypass/disable security
    [GeneratedRegex(@"(?i)(bypass|disable|ignore|skip|turn\s+off)\s+(security|policy|policies|guardrails?|restrictions?|rules?|filter)", RegexOptions.IgnoreCase)]
    private static partial Regex BypassSecurityRegex();

    // Jailbreak patterns
    [GeneratedRegex(@"(?i)(jailbreak|dan\s+mode|developer\s+mode|unrestricted\s+mode|act\s+as\s+(unrestricted|unfiltered|unconstrained))", RegexOptions.IgnoreCase)]
    private static partial Regex JailbreakRegex();

    // "Ignore instructions" patterns
    [GeneratedRegex(@"(?i)ignore\s+(?:(?:all|your|the|any|my|these|those)\s+)*(previous|prior|above|earlier|last)?\s*instructions?", RegexOptions.IgnoreCase)]
    private static partial Regex IgnoreInstructionsRegex();

    // Override/hidden prompts
    [GeneratedRegex(@"(?i)(override|replace|disregard|forget)\s+(your\s+)?(instructions?|context|rules?|guidelines?|training)", RegexOptions.IgnoreCase)]
    private static partial Regex OverrideInstructionsRegex();

    // Developer/system message injection
    [GeneratedRegex(@"(?i)\[(developer|system|hidden|admin|root)\s*(message|prompt|instruction|mode)\]", RegexOptions.IgnoreCase)]
    private static partial Regex InjectedMessageRegex();

    // Maximum number of characters scanned per call to prevent DoS (VULN-05).
    private const int MaxInputLength = 65_536;

    /// <summary>
    /// Scans the specified text for prompt injection indicators.
    /// </summary>
    /// <param name="input">The text to scan. May be <see langword="null"/> or empty.</param>
    /// <returns>A <see cref="GovernAIRiskResult"/> describing the detected risk.</returns>
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
        string? reason = null;

        if (SecretExtractionRegex().IsMatch(input))
        {
            matchedPatterns.Add("SecretExtraction");
            if (maxScore < 95) { maxScore = 95; reason = "Critical: secret extraction attempt detected"; }
        }

        if (JailbreakRegex().IsMatch(input))
        {
            matchedPatterns.Add("Jailbreak");
            if (maxScore < 80) { maxScore = 80; reason = "High: jailbreak attempt detected"; }
        }

        if (SystemPromptExtractionRegex().IsMatch(input))
        {
            matchedPatterns.Add("SystemPromptExtraction");
            if (maxScore < 75) { maxScore = 75; reason = "High: system prompt extraction attempt detected"; }
        }

        if (BypassSecurityRegex().IsMatch(input))
        {
            matchedPatterns.Add("BypassSecurity");
            if (maxScore < 75) { maxScore = 75; reason = "High: attempt to bypass security controls"; }
        }

        if (IgnoreInstructionsRegex().IsMatch(input))
        {
            matchedPatterns.Add("IgnoreInstructions");
            if (maxScore < 50) { maxScore = 50; reason = "Medium: suspicious instruction-override pattern"; }
        }

        if (OverrideInstructionsRegex().IsMatch(input))
        {
            matchedPatterns.Add("OverrideInstructions");
            if (maxScore < 50) { maxScore = 50; reason = "Medium: suspicious instruction-override pattern"; }
        }

        if (InjectedMessageRegex().IsMatch(input))
        {
            matchedPatterns.Add("InjectedMessage");
            if (maxScore < 50) { maxScore = 50; reason = "Medium: suspicious injected message pattern"; }
        }

        return new GovernAIRiskResult
        {
            RiskScore = maxScore,
            RiskLevel = RiskScoreCalculator.ScoreToLevel(maxScore),
            RiskCategory = matchedPatterns.Count > 0 ? "PromptInjection" : null,
            Reason = reason,
            MatchedPatterns = matchedPatterns.Count > 0 ? matchedPatterns.AsReadOnly() : null
        };
    }
}
