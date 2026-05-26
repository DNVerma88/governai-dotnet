using GovernAI.Abstractions;

namespace GovernAI.Abstractions.Tests.Models;

public sealed class GovernAIEventTests
{
    [Fact]
    public void Create_WithRequiredFields_ShouldInitializeCorrectly()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        var @event = new GovernAIEvent
        {
            EventId = "abc123",
            ApplicationName = "TestApp",
            OperationName = "GenerateSummary",
            Success = true,
            TimestampUtc = timestamp
        };

        // Assert
        Assert.Equal("abc123", @event.EventId);
        Assert.Equal("TestApp", @event.ApplicationName);
        Assert.Equal("GenerateSummary", @event.OperationName);
        Assert.True(@event.Success);
        Assert.Equal(timestamp, @event.TimestampUtc);
    }

    [Fact]
    public void Create_WithAllFields_ShouldInitializeAllPropertiesCorrectly()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var metadata = new Dictionary<string, string> { ["key1"] = "value1", ["key2"] = "value2" };

        // Act
        var @event = new GovernAIEvent
        {
            EventId = "evt-001",
            TraceId = "trace-001",
            CorrelationId = "corr-001",
            ApplicationName = "Enterprise.Api",
            EnvironmentName = "Production",
            TenantId = "tenant-001",
            UserId = "user-001",
            AgentName = "ReportAgent",
            OperationName = "GenerateSummary",
            ModelProvider = "AzureOpenAI",
            ModelName = "gpt-4.1",
            PromptHash = "sha256-prompt-hash",
            ResponseHash = "sha256-response-hash",
            InputTokens = 1200,
            OutputTokens = 300,
            TotalTokens = 1500,
            RiskScore = 10,
            RiskLevel = GovernAIRiskLevel.Low,
            RiskCategory = "None",
            PolicyDecision = GovernAIPolicyDecisionType.Allow,
            PolicyReason = "Low risk prompt",
            DurationMs = 850,
            Success = true,
            TimestampUtc = timestamp,
            Metadata = metadata
        };

        // Assert
        Assert.Equal("evt-001", @event.EventId);
        Assert.Equal("trace-001", @event.TraceId);
        Assert.Equal("corr-001", @event.CorrelationId);
        Assert.Equal("Enterprise.Api", @event.ApplicationName);
        Assert.Equal("Production", @event.EnvironmentName);
        Assert.Equal("tenant-001", @event.TenantId);
        Assert.Equal("user-001", @event.UserId);
        Assert.Equal("ReportAgent", @event.AgentName);
        Assert.Equal("GenerateSummary", @event.OperationName);
        Assert.Equal("AzureOpenAI", @event.ModelProvider);
        Assert.Equal("gpt-4.1", @event.ModelName);
        Assert.Equal("sha256-prompt-hash", @event.PromptHash);
        Assert.Equal("sha256-response-hash", @event.ResponseHash);
        Assert.Equal(1200, @event.InputTokens);
        Assert.Equal(300, @event.OutputTokens);
        Assert.Equal(1500, @event.TotalTokens);
        Assert.Equal(10, @event.RiskScore);
        Assert.Equal(GovernAIRiskLevel.Low, @event.RiskLevel);
        Assert.Equal("None", @event.RiskCategory);
        Assert.Equal(GovernAIPolicyDecisionType.Allow, @event.PolicyDecision);
        Assert.Equal("Low risk prompt", @event.PolicyReason);
        Assert.Equal(850, @event.DurationMs);
        Assert.True(@event.Success);
        Assert.Equal(timestamp, @event.TimestampUtc);
        Assert.NotNull(@event.Metadata);
        Assert.Equal("value1", @event.Metadata["key1"]);
        Assert.Equal("value2", @event.Metadata["key2"]);
    }

    [Fact]
    public void OptionalFields_ShouldDefaultToNull_WhenNotProvided()
    {
        // Act
        var @event = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = false,
            TimestampUtc = DateTimeOffset.UtcNow
        };

        // Assert
        Assert.Null(@event.TraceId);
        Assert.Null(@event.CorrelationId);
        Assert.Null(@event.EnvironmentName);
        Assert.Null(@event.TenantId);
        Assert.Null(@event.UserId);
        Assert.Null(@event.AgentName);
        Assert.Null(@event.ModelProvider);
        Assert.Null(@event.ModelName);
        Assert.Null(@event.PromptHash);
        Assert.Null(@event.ResponseHash);
        Assert.Null(@event.InputTokens);
        Assert.Null(@event.OutputTokens);
        Assert.Null(@event.TotalTokens);
        Assert.Null(@event.RiskCategory);
        Assert.Null(@event.PolicyReason);
        Assert.Null(@event.ErrorCode);
        Assert.Null(@event.ErrorMessage);
        Assert.Null(@event.Metadata);
    }

    [Fact]
    public void NumericFields_ShouldDefaultToZero_WhenNotProvided()
    {
        // Act
        var @event = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };

        // Assert
        Assert.Equal(0, @event.RiskScore);
        Assert.Equal(0L, @event.DurationMs);
    }

    [Fact]
    public void RiskLevel_ShouldDefaultToNone_WhenNotProvided()
    {
        // Act
        var @event = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };

        // Assert
        Assert.Equal(GovernAIRiskLevel.None, @event.RiskLevel);
    }

    [Fact]
    public void PolicyDecision_ShouldDefaultToAllow_WhenNotProvided()
    {
        // Act
        var @event = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };

        // Assert
        Assert.Equal(GovernAIPolicyDecisionType.Allow, @event.PolicyDecision);
    }

    [Fact]
    public void TwoEvents_WithIdenticalValues_ShouldBeEqual()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var event1 = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = timestamp
        };

        var event2 = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = timestamp
        };

        // Assert
        Assert.Equal(event1, event2);
    }

    [Fact]
    public void TwoEvents_WithDifferentEventIds_ShouldNotBeEqual()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var event1 = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = timestamp
        };

        var event2 = new GovernAIEvent
        {
            EventId = "evt-002",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = timestamp
        };

        // Assert
        Assert.NotEqual(event1, event2);
    }

    [Fact]
    public void Record_ShouldBeImmutable_AfterConstruction()
    {
        // Arrange
        var @event = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };

        // Act — with-expression creates a new record; original is unchanged
        var modified = @event with { ApplicationName = "OtherApp" };

        // Assert
        Assert.Equal("TestApp", @event.ApplicationName);
        Assert.Equal("OtherApp", modified.ApplicationName);
        Assert.Equal(@event.EventId, modified.EventId);
    }

    [Fact]
    public void FailedEvent_ShouldCarryErrorDetails()
    {
        // Act
        var @event = new GovernAIEvent
        {
            EventId = "evt-fail-001",
            ApplicationName = "TestApp",
            OperationName = "GenerateSummary",
            Success = false,
            ErrorCode = "AI_TIMEOUT",
            ErrorMessage = "The AI operation timed out.",
            TimestampUtc = DateTimeOffset.UtcNow
        };

        // Assert
        Assert.False(@event.Success);
        Assert.Equal("AI_TIMEOUT", @event.ErrorCode);
        Assert.Equal("The AI operation timed out.", @event.ErrorMessage);
    }

    [Theory]
    [InlineData(GovernAIRiskLevel.None, GovernAIPolicyDecisionType.Allow, 0)]
    [InlineData(GovernAIRiskLevel.Low, GovernAIPolicyDecisionType.Allow, 10)]
    [InlineData(GovernAIRiskLevel.Medium, GovernAIPolicyDecisionType.Allow, 40)]
    [InlineData(GovernAIRiskLevel.High, GovernAIPolicyDecisionType.Review, 75)]
    [InlineData(GovernAIRiskLevel.Critical, GovernAIPolicyDecisionType.Deny, 100)]
    public void RiskLevelAndDecision_CombinationsAreValid(
        GovernAIRiskLevel riskLevel, GovernAIPolicyDecisionType decision, int riskScore)
    {
        // Act
        var @event = new GovernAIEvent
        {
            EventId = "evt-001",
            ApplicationName = "TestApp",
            OperationName = "TestOp",
            RiskLevel = riskLevel,
            PolicyDecision = decision,
            RiskScore = riskScore,
            Success = true,
            TimestampUtc = DateTimeOffset.UtcNow
        };

        // Assert
        Assert.Equal(riskLevel, @event.RiskLevel);
        Assert.Equal(decision, @event.PolicyDecision);
        Assert.Equal(riskScore, @event.RiskScore);
    }
}
