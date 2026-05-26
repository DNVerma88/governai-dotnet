using Microsoft.AspNetCore.Http;

namespace GovernAI.AspNetCore;

/// <summary>
/// Middleware that manages the GovernAI correlation ID lifecycle.
/// Reads an existing <c>X-Correlation-Id</c> header from the request, or generates a new one.
/// Writes the correlation ID to the response header.
/// </summary>
public sealed class GovernAIMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of <see cref="GovernAIMiddleware"/>.
    /// </summary>
    public GovernAIMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    /// <summary>
    /// Processes an HTTP request, ensuring a correlation ID is present on both request and response.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Sanitize the incoming header to prevent log injection via control characters
        // (e.g. \r\n could enable HTTP response splitting and corrupt JSON Lines log files).
        var rawHeader = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();
        var correlationId = SanitizeCorrelationId(rawHeader);
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        // Store in Items so downstream code can access without re-reading the header
        context.Items[CorrelationIdHeader] = correlationId;

        // Write to response header before sending response; guard against already-started response
        if (!context.Response.HasStarted)
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
        }

        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
            }
            return Task.CompletedTask;
        });

        await _next(context).ConfigureAwait(false);
    }

    /// <summary>
    /// Strips control characters from a correlation ID to prevent log injection,
    /// and caps length to 128 characters.
    /// </summary>
    private static string? SanitizeCorrelationId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        Span<char> buffer = stackalloc char[128];
        var len = 0;
        foreach (var ch in value)
        {
            if (len >= 128) break;
            if (!char.IsControl(ch))
                buffer[len++] = ch;
        }

        var result = new string(buffer[..len]);
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }
}