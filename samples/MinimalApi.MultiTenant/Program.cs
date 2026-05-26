using GovernAI.Abstractions;
using GovernAI.AspNetCore;
using GovernAI.Core;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGovernAI(options =>
{
    options.ApplicationName = "MinimalApi.MultiTenant";
    options.EnvironmentName = builder.Environment.EnvironmentName;
});

// Rate limiting: protect the AI endpoint from flooding attacks (VULN-12).
builder.Services.AddRateLimiter(opts =>
    opts.AddFixedWindowLimiter("chat", policy =>
    {
        policy.PermitLimit = 60;
        policy.Window = TimeSpan.FromMinutes(1);
        policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        policy.QueueLimit = 5;
    }));

var app = builder.Build();
app.UseGovernAI();
app.UseRateLimiter();

// Multi-tenant chat: tenant resolved from authenticated claims (tenant_id / tid) first,
// then falls back to the X-Tenant-Id header for unauthenticated M2M scenarios.
// User ID is resolved from authenticated JWT claims (sub / nameidentifier) by
// HttpContextUserResolver — never from the client-supplied request body (VULN-08).
app.MapPost("/api/chat", async (ChatRequest request, GovernAIRuntime governAI) =>
{
    var context = new GovernAIContext
    {
        OperationName = "chat",
        ModelProvider = "fake",
        ModelName = "fake-gpt",
        Prompt = request.Prompt
        // TenantId and UserId are intentionally omitted here so they are resolved
        // from authenticated claims by HttpContextTenantResolver / HttpContextUserResolver.
    };

    try
    {
        var response = await governAI.TrackAsync(
            context,
            async ct => await FakeAiCallAsync(request.Prompt, ct));

        return Results.Ok(new { response });
    }
    catch (GovernAIDeniedException)
    {
        // Return a generic message so internal detection signatures are not revealed (VULN-09).
        return Results.Problem(
            detail: "Your request could not be processed.",
            statusCode: StatusCodes.Status403Forbidden,
            title: "Request denied");
    }
}).RequireRateLimiting("chat");

// Audit event inspection endpoint: restricted to Development to prevent
// unauthenticated exposure of the full governance log in production (VULN-02).
if (app.Environment.IsDevelopment())
{
    app.MapGet("/api/events", (GovernAI.Abstractions.IGovernAIExporter exporter) =>
    {
        return exporter is GovernAI.Core.InMemoryExporter mem
            ? Results.Ok(mem.Events)
            : Results.Ok(Array.Empty<GovernAI.Abstractions.GovernAIEvent>());
    });
}

app.Run();

static Task<string> FakeAiCallAsync(string prompt, CancellationToken cancellationToken)
{
    return Task.FromResult($"[Tenant-aware] Simulated AI response to: {prompt[..Math.Min(50, prompt.Length)]}...");
}

internal sealed record ChatRequest(string Prompt);
