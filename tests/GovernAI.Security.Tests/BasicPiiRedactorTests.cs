using GovernAI.Security;

namespace GovernAI.Security.Tests;

public sealed class BasicPiiRedactorTests
{
    private readonly BasicPiiRedactor _redactor = new();

    [Fact]
    public void Redact_NullInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, _redactor.Redact(null));
    }

    [Fact]
    public void Redact_EmptyInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, _redactor.Redact(string.Empty));
    }

    [Fact]
    public void Redact_EmailAddress_IsRedacted()
    {
        var result = _redactor.Redact("Contact us at user@example.com for support");
        Assert.DoesNotContain("user@example.com", result);
        Assert.Contains("[REDACTED_EMAIL]", result);
    }

    [Fact]
    public void Redact_PhoneNumber_IsRedacted()
    {
        var result = _redactor.Redact("Call 555-867-5309 to reach us");
        Assert.DoesNotContain("555-867-5309", result);
        Assert.Contains("[REDACTED_PHONE]", result);
    }

    [Fact]
    public void Redact_BearerToken_IsRedacted()
    {
        var result = _redactor.Redact("Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9abcdefabcdef");
        Assert.DoesNotContain("eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9abcdefabcdef", result);
    }

    [Fact]
    public void Redact_ApiKey_IsRedacted()
    {
        var result = _redactor.Redact("The API key is api_key=sk-abcdefghijklmnopqrstuvwxyz123456");
        Assert.DoesNotContain("sk-abcdefghijklmnopqrstuvwxyz123456", result);
        Assert.Contains("[REDACTED_API_KEY]", result);
    }

    [Fact]
    public void Redact_Password_IsRedacted()
    {
        var result = _redactor.Redact("Set password=MyS3cr3tP@ssword! to login");
        Assert.DoesNotContain("MyS3cr3tP@ssword!", result);
        Assert.Contains("[REDACTED_SECRET]", result);
    }

    [Fact]
    public void Redact_AzureStorageConnectionString_IsRedacted()
    {
        var result = _redactor.Redact(
            "DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=abcdefghijklmnopqrstuvwxyz123456==;EndpointSuffix=core.windows.net");
        Assert.DoesNotContain("AccountKey=abcdefghijklmnopqrstuvwxyz123456==", result);
        Assert.Contains("[REDACTED_CONNECTION_STRING]", result);
    }

    [Fact]
    public void Redact_JwtToken_IsRedacted()
    {
        var result = _redactor.Redact("Token: eyJhbGciOiJSUzI1NiJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
        Assert.DoesNotContain("eyJhbGciOiJSUzI1NiJ9", result);
        Assert.Contains("[REDACTED_TOKEN]", result);
    }

    [Fact]
    public void Redact_CreditCardLikeNumber_IsRedacted()
    {
        var result = _redactor.Redact("My card is 4111 1111 1111 1111 and expires tomorrow");
        Assert.DoesNotContain("4111 1111 1111 1111", result);
        Assert.Contains("[REDACTED_CARD]", result);
    }

    [Fact]
    public void Redact_SafeText_IsUnchanged()
    {
        const string input = "Please summarise this document about climate change.";
        Assert.Equal(input, _redactor.Redact(input));
    }

    [Fact]
    public void Redact_MultiplePatterns_AllRedacted()
    {
        var result = _redactor.Redact("Email: admin@corp.com, Password: secret123");
        Assert.DoesNotContain("admin@corp.com", result);
        Assert.Contains("[REDACTED_EMAIL]", result);
        Assert.Contains("[REDACTED_SECRET]", result);
    }
}
