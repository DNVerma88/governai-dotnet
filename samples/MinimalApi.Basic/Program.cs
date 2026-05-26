using GovernAI.Abstractions;
using GovernAI.AspNetCore;
using GovernAI.Core;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Register GovernAI with a CompositeExporter: Console + InMemory
// This demonstrates the ConsoleExporter alongside the InMemoryExporter.
var inMemory = new InMemoryExporter(1000);
builder.Services.AddSingleton<IGovernAIExporter>(
    new CompositeExporter([new ConsoleExporter(), inMemory]));
builder.Services.AddSingleton(inMemory); // allow direct injection for /api/events

builder.Services.AddGovernAI(options =>
{
    options.ApplicationName = "MinimalApi.Basic";
    options.EnvironmentName = builder.Environment.EnvironmentName;
    options.EnablePromptHashing = true;
    options.EnableResponseHashing = true;
    options.AllowRawPromptCapture = false;
    options.AllowRawResponseCapture = false;
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

app.MapPost("/api/chat", async (ChatRequest request, GovernAIRuntime governAI) =>
{
    var context = new GovernAIContext
    {
        OperationName = "chat",
        ModelProvider = "fake",
        ModelName = "fake-gpt",
        Prompt = request.Prompt
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
    app.MapGet("/api/events", (InMemoryExporter exporter) =>
        Results.Ok(exporter.Events));
}

app.Run();

static Task<string> FakeAiCallAsync(string prompt, CancellationToken cancellationToken)
{
    return Task.FromResult($"Fake AI response for: {prompt}");
}

internal sealed record ChatRequest(string Prompt);
