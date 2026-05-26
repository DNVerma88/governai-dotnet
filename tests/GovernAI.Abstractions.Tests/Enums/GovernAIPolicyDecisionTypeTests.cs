using GovernAI.Abstractions;

namespace GovernAI.Abstractions.Tests.Enums;

public sealed class GovernAIPolicyDecisionTypeTests
{
    [Fact]
    public void Allow_ShouldHaveValue_Zero()
    {
        Assert.Equal(0, (int)GovernAIPolicyDecisionType.Allow);
    }

    [Fact]
    public void Review_ShouldHaveValue_One()
    {
        Assert.Equal(1, (int)GovernAIPolicyDecisionType.Review);
    }

    [Fact]
    public void Deny_ShouldHaveValue_Two()
    {
        Assert.Equal(2, (int)GovernAIPolicyDecisionType.Deny);
    }

    [Fact]
    public void Enum_ShouldHaveExactly_ThreeMembers()
    {
        var values = Enum.GetValues<GovernAIPolicyDecisionType>();
        Assert.Equal(3, values.Length);
    }

    [Theory]
    [InlineData(GovernAIPolicyDecisionType.Allow, "Allow")]
    [InlineData(GovernAIPolicyDecisionType.Review, "Review")]
    [InlineData(GovernAIPolicyDecisionType.Deny, "Deny")]
    public void ToString_ShouldReturnExpectedName(GovernAIPolicyDecisionType value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }
}
