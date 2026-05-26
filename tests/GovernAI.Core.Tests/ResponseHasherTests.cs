using GovernAI.Core;

namespace GovernAI.Core.Tests;

public sealed class ResponseHasherTests
{
    [Fact]
    public void Hash_NullInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, ResponseHasher.Hash(null));
    }

    [Fact]
    public void Hash_EmptyInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, ResponseHasher.Hash(string.Empty));
    }

    [Fact]
    public void Hash_ValidInput_ReturnsLowercaseHexString()
    {
        var hash = ResponseHasher.Hash("AI response text");
        Assert.NotEmpty(hash);
        Assert.Equal(64, hash.Length);
        Assert.Equal(hash, hash.ToLowerInvariant());
    }

    [Fact]
    public void Hash_SameInput_ReturnsSameHash()
    {
        const string input = "The response was successful.";
        Assert.Equal(ResponseHasher.Hash(input), ResponseHasher.Hash(input));
    }

    [Fact]
    public void Hash_DifferentInputs_ReturnDifferentHashes()
    {
        Assert.NotEqual(ResponseHasher.Hash("response A"), ResponseHasher.Hash("response B"));
    }
}
