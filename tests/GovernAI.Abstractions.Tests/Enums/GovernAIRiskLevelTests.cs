using GovernAI.Abstractions;

namespace GovernAI.Abstractions.Tests.Enums;

public sealed class GovernAIRiskLevelTests
{
    [Fact]
    public void None_ShouldHaveValue_Zero()
    {
        Assert.Equal(0, (int)GovernAIRiskLevel.None);
    }

    [Fact]
    public void Low_ShouldHaveValue_One()
    {
        Assert.Equal(1, (int)GovernAIRiskLevel.Low);
    }

    [Fact]
    public void Medium_ShouldHaveValue_Two()
    {
        Assert.Equal(2, (int)GovernAIRiskLevel.Medium);
    }

    [Fact]
    public void High_ShouldHaveValue_Three()
    {
        Assert.Equal(3, (int)GovernAIRiskLevel.High);
    }

    [Fact]
    public void Critical_ShouldHaveValue_Four()
    {
        Assert.Equal(4, (int)GovernAIRiskLevel.Critical);
    }

    [Fact]
    public void Enum_ShouldHaveExactly_FiveMembers()
    {
        var values = Enum.GetValues<GovernAIRiskLevel>();
        Assert.Equal(5, values.Length);
    }

    [Fact]
    public void RiskLevels_ShouldBeOrdered_LowToHigh()
    {
        Assert.True(GovernAIRiskLevel.None < GovernAIRiskLevel.Low);
        Assert.True(GovernAIRiskLevel.Low < GovernAIRiskLevel.Medium);
        Assert.True(GovernAIRiskLevel.Medium < GovernAIRiskLevel.High);
        Assert.True(GovernAIRiskLevel.High < GovernAIRiskLevel.Critical);
    }

    [Theory]
    [InlineData(GovernAIRiskLevel.None, "None")]
    [InlineData(GovernAIRiskLevel.Low, "Low")]
    [InlineData(GovernAIRiskLevel.Medium, "Medium")]
    [InlineData(GovernAIRiskLevel.High, "High")]
    [InlineData(GovernAIRiskLevel.Critical, "Critical")]
    public void ToString_ShouldReturnExpectedName(GovernAIRiskLevel value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }
}
