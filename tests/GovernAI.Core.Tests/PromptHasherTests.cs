using GovernAI.Core;

namespace GovernAI.Core.Tests;

public sealed class PromptHasherTests
{
    [Fact]
    public void Hash_NullInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, PromptHasher.Hash(null));
    }

    [Fact]
    public void Hash_EmptyInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, PromptHasher.Hash(string.Empty));
    }

    [Fact]
    public void Hash_ValidInput_ReturnsLowercaseHexString()
    {
        var hash = PromptHasher.Hash("Hello, World!");
        Assert.NotEmpty(hash);
        Assert.Equal(64, hash.Length); // SHA-256 produces 32 bytes = 64 hex chars
        Assert.Equal(hash, hash.ToLowerInvariant());
    }

    [Fact]
    public void Hash_SameInput_ReturnsSameHash()
    {
        const string input = "Summarise the document";
        var hash1 = PromptHasher.Hash(input);
        var hash2 = PromptHasher.Hash(input);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_DifferentInputs_ReturnDifferentHashes()
    {
        var hash1 = PromptHasher.Hash("prompt one");
        var hash2 = PromptHasher.Hash("prompt two");
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Hash_KnownValue_IsDeterministicAndConsistent()
    {
        // Same input should always produce the same 64-char lowercase hex hash
        var hash1 = PromptHasher.Hash("abc");
        var hash2 = PromptHasher.Hash("abc");
        Assert.Equal(64, hash1.Length);
        Assert.Equal(hash1, hash2);
        Assert.Equal(hash1.ToLowerInvariant(), hash1);
    }
}
