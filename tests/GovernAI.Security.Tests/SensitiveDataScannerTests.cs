using GovernAI.Abstractions;
using GovernAI.Security;

namespace GovernAI.Security.Tests;

public sealed class SensitiveDataScannerTests
{
    private readonly SensitiveDataScanner _scanner = new();

    [Fact]
    public void Scan_NullInput_ReturnsNoRisk()
    {
        var result = _scanner.Scan(null);
        Assert.Equal(0, result.RiskScore);
        Assert.Equal(GovernAIRiskLevel.None, result.RiskLevel);
    }

    [Fact]
    public void Scan_EmptyInput_ReturnsNoRisk()
    {
        var result = _scanner.Scan(string.Empty);
        Assert.Equal(0, result.RiskScore);
        Assert.Equal(GovernAIRiskLevel.None, result.RiskLevel);
    }

    [Fact]
    public void Scan_SafeText_ReturnsNoRisk()
    {
        var result = _scanner.Scan("What is the capital of France?");
        Assert.Equal(0, result.RiskScore);
        Assert.Equal(GovernAIRiskLevel.None, result.RiskLevel);
        Assert.Null(result.MatchedPatterns);
    }

    [Fact]
    public void Scan_EmailAddress_ReturnsLowRisk()
    {
        var result = _scanner.Scan("Contact alice@example.com for details");
        Assert.True(result.RiskScore > 0);
        Assert.Equal(GovernAIRiskLevel.Low, result.RiskLevel);
        Assert.Contains("Email", result.MatchedPatterns!);
    }

    [Fact]
    public void Scan_PhoneNumber_ReturnsLowRisk()
    {
        var result = _scanner.Scan("Call us at 555-867-5309");
        Assert.Equal(GovernAIRiskLevel.Low, result.RiskLevel);
        Assert.Contains("PhoneNumber", result.MatchedPatterns!);
    }

    [Fact]
    public void Scan_BearerToken_ReturnsHighRisk()
    {
        var result = _scanner.Scan("Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9abcdefabcdef");
        Assert.True(result.RiskScore >= 75);
        Assert.True(result.RiskLevel >= GovernAIRiskLevel.High);
        Assert.Equal("SensitiveData", result.RiskCategory);
    }

    [Fact]
    public void Scan_ApiKey_ReturnsHighRisk()
    {
        var result = _scanner.Scan("api_key=sk-abcdefghijklmnopqrstuvwxyz123456");
        Assert.True(result.RiskScore >= 75);
        Assert.True(result.RiskLevel >= GovernAIRiskLevel.High);
        Assert.Contains("ApiKey", result.MatchedPatterns!);
    }

    [Fact]
    public void Scan_CreditCard_ReturnsHighRisk()
    {
        var result = _scanner.Scan("My card is 4111 1111 1111 1111");
        Assert.True(result.RiskScore >= 75);
        Assert.True(result.RiskLevel >= GovernAIRiskLevel.High);
        Assert.Contains("CreditCard", result.MatchedPatterns!);
    }

    [Fact]
    public void Scan_Password_ReturnsCriticalRisk()
    {
        var result = _scanner.Scan("password=MyS3cr3tP@ss!");
        Assert.True(result.RiskScore >= 86);
        Assert.Equal(GovernAIRiskLevel.Critical, result.RiskLevel);
        Assert.Contains("Password", result.MatchedPatterns!);
    }

    [Fact]
    public void Scan_AzureStorageConnectionString_ReturnsCriticalRisk()
    {
        var result = _scanner.Scan(
            "DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=abc123==;EndpointSuffix=core.windows.net");
        Assert.True(result.RiskScore >= 86);
        Assert.Equal(GovernAIRiskLevel.Critical, result.RiskLevel);
        Assert.Contains("ConnectionString", result.MatchedPatterns!);
    }

    [Fact]
    public void Scan_MultiplePatterns_ReturnsHighestRisk()
    {
        // Email (Low=20) + Bearer (High=75) → should return High
        var result = _scanner.Scan("Email alice@example.com. Token: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9abcdefabcdef");
        Assert.True(result.RiskScore >= 75);
        Assert.True(result.RiskLevel >= GovernAIRiskLevel.High);
        Assert.Contains("Email", result.MatchedPatterns!);
        Assert.Contains("BearerToken/JWT", result.MatchedPatterns!);
    }

    [Fact]
    public void Scan_RiskCategory_IsSensitiveData()
    {
        var result = _scanner.Scan("api_key=sk-abcdefghijklmnopqrstuvwxyz123456");
        Assert.Equal("SensitiveData", result.RiskCategory);
    }
}
