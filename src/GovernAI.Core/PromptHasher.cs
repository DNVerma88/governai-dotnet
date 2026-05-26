using System.Security.Cryptography;
using System.Text;

namespace GovernAI.Core;

/// <summary>
/// Computes a SHA-256 or HMAC-SHA256 hash of a prompt string.
/// </summary>
public static class PromptHasher
{
    /// <summary>
    /// Computes a hash of the specified prompt text.
    /// When <paramref name="key"/> is provided, uses HMAC-SHA256 to prevent rainbow-table
    /// preimage attacks. Without a key, falls back to plain SHA-256.
    /// </summary>
    /// <param name="prompt">The prompt to hash. May be <see langword="null"/> or empty.</param>
    /// <param name="key">
    /// Optional HMAC key. Must be at least 16 bytes. When <see langword="null"/> or empty,
    /// plain SHA-256 is used. Configure via <see cref="GovernAIOptions.PromptHashKey"/>.
    /// </param>
    /// <returns>
    /// A lowercase hexadecimal hash string (64 chars), or <see cref="string.Empty"/> when
    /// <paramref name="prompt"/> is <see langword="null"/> or empty.
    /// </returns>
    public static string Hash(string? prompt, byte[]? key = null)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            return string.Empty;
        }

        var inputBytes = Encoding.UTF8.GetBytes(prompt);

        if (key is { Length: > 0 })
        {
            // Static one-shot method avoids allocating + disposing a HMACSHA256 instance
            // on every call, which improves throughput on the hot path.
            return Convert.ToHexString(HMACSHA256.HashData(key, inputBytes)).ToLowerInvariant();
        }

        return Convert.ToHexString(SHA256.HashData(inputBytes)).ToLowerInvariant();
    }
}
