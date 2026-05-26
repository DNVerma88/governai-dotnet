using GovernAI.Abstractions;

namespace GovernAI.Abstractions.Tests.Models;

public sealed class GovernAIContextTests
{
    [Fact]
    public void Create_WithRequiredField_ShouldInitializeCorrectly()
    {
        // Act
        var context = new GovernAIContext
        {
            OperationName = "GenerateSummary"
        };

        // Assert
        Assert.Equal("GenerateSummary", context.OperationName);
    }

    [Fact]
    public void Create_WithAllFields_ShouldInitializeAllPropertiesCorrectly()
    {
        // Arrange
        var metadata = new Dictionary<string, string> { ["env"] = "test" };

        // Act
        var context = new GovernAIContext
        {
            TraceId = "trace-001",
            CorrelationId = "corr-001",
            ApplicationName = "TestApp",
            EnvironmentName = "Development",
            TenantId = "tenant-001",
            UserId = "user-001",
            AgentName = "ReportAgent",
            OperationName = "GenerateSummary",
            ModelProvider = "AzureOpenAI",
            ModelName = "gpt-4.1",
            Prompt = "Summarise the following text...",
            Response = "Here is a summary...",
            InputTokens = 100,
            OutputTokens = 50,
            Metadata = metadata
        };

        // Assert
        Assert.Equal("trace-001", context.TraceId);
        Assert.Equal("corr-001", context.CorrelationId);
        Assert.Equal("TestApp", context.ApplicationName);
        Assert.Equal("Development", context.EnvironmentName);
        Assert.Equal("tenant-001", context.TenantId);
        Assert.Equal("user-001", context.UserId);
        Assert.Equal("ReportAgent", context.AgentName);
        Assert.Equal("GenerateSummary", context.OperationName);
        Assert.Equal("AzureOpenAI", context.ModelProvider);
        Assert.Equal("gpt-4.1", context.ModelName);
        Assert.Equal("Summarise the following text...", context.Prompt);
        Assert.Equal("Here is a summary...", context.Response);
        Assert.Equal(100, context.InputTokens);
        Assert.Equal(50, context.OutputTokens);
        Assert.NotNull(context.Metadata);
        Assert.Equal("test", context.Metadata["env"]);
    }

    [Fact]
    public void OptionalFields_ShouldDefaultToNull_WhenNotProvided()
    {
        // Act
        var context = new GovernAIContext
        {
            OperationName = "TestOp"
        };

        // Assert
        Assert.Null(context.TraceId);
        Assert.Null(context.CorrelationId);
        Assert.Null(context.ApplicationName);
        Assert.Null(context.EnvironmentName);
        Assert.Null(context.AgentName);
        Assert.Null(context.ModelProvider);
        Assert.Null(context.ModelName);
        Assert.Null(context.TenantId);
        Assert.Null(context.UserId);
        Assert.Null(context.Prompt);
        Assert.Null(context.Response);
        Assert.Null(context.InputTokens);
        Assert.Null(context.OutputTokens);
        Assert.Null(context.Metadata);
    }

    [Fact]
    public void TwoContexts_WithIdenticalValues_ShouldBeEqual()
    {
        // Arrange
        var context1 = new GovernAIContext { OperationName = "TestOp", TenantId = "t1" };
        var context2 = new GovernAIContext { OperationName = "TestOp", TenantId = "t1" };

        // Assert
        Assert.Equal(context1, context2);
    }

    [Fact]
    public void Record_ShouldBeImmutable_AfterConstruction()
    {
        // Arrange
        var context = new GovernAIContext { OperationName = "OriginalOp" };

        // Act — with-expression creates a new record; original is unchanged
        var modified = context with { OperationName = "ModifiedOp" };

        // Assert
        Assert.Equal("OriginalOp", context.OperationName);
        Assert.Equal("ModifiedOp", modified.OperationName);
    }
}
