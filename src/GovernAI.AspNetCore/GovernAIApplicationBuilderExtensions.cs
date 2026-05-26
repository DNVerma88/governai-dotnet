using Microsoft.AspNetCore.Builder;

namespace GovernAI.AspNetCore;

/// <summary>
/// Extension methods for adding GovernAI middleware to the application pipeline.
/// </summary>
public static class GovernAIApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="GovernAIMiddleware"/> to the application pipeline.
    /// </summary>
    /// <remarks>
    /// This middleware adds correlation ID handling. It is recommended to place it
    /// early in the pipeline before routing and authentication middleware.
    /// </remarks>
    /// <param name="app">The application builder.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> for chaining.</returns>
    public static IApplicationBuilder UseGovernAI(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<GovernAIMiddleware>();
    }
}
