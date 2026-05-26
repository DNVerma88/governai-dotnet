using GovernAI.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace GovernAI.AspNetCore.Tests;

public sealed class GovernAIMiddlewareTests
{
    [Fact]
    public async Task Middleware_ResponseIncludesCorrelationIdHeader()
    {
        var context = new DefaultHttpContext();
        var middleware = new GovernAIMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.True(context.Response.Headers.ContainsKey("X-Correlation-Id"));
    }

    [Fact]
    public async Task Middleware_ExistingCorrelationId_IsPreserved()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = "test-correlation-123";
        var middleware = new GovernAIMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        var correlationId = context.Response.Headers["X-Correlation-Id"].ToString();
        Assert.Equal("test-correlation-123", correlationId);
    }

    [Fact]
    public async Task Middleware_MissingCorrelationId_GeneratesNewGuid()
    {
        var context = new DefaultHttpContext();
        var middleware = new GovernAIMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        var correlationId = context.Response.Headers["X-Correlation-Id"].ToString();
        Assert.NotNull(correlationId);
        Assert.NotEmpty(correlationId);
        // Generated IDs must be 32-char GUIDs (no hyphens)
        Assert.Equal(32, correlationId.Length);
    }

    [Fact]
    public async Task Middleware_StoresCorrelationIdInItems()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = "item-test-id";
        var middleware = new GovernAIMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.Equal("item-test-id", context.Items["X-Correlation-Id"]);
    }

    [Fact]
    public async Task Middleware_DoesNotReadRequestBody()
    {
        var context = new DefaultHttpContext();
        var bodyStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("body content"));
        context.Request.Body = bodyStream;
        var middleware = new GovernAIMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        // Body position should be 0 — middleware did not read the body
        Assert.Equal(0, bodyStream.Position);
    }

    [Fact]
    public async Task Middleware_CallsNextDelegate()
    {
        var nextCalled = false;
        var context = new DefaultHttpContext();
        var middleware = new GovernAIMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }
}
