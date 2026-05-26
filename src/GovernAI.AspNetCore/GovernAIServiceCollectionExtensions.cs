using GovernAI.Abstractions;
using GovernAI.Core;
using GovernAI.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GovernAI.AspNetCore;

/// <summary>
/// Extension methods for registering GovernAI services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class GovernAIServiceCollectionExtensions
{
    /// <summary>
    /// Registers GovernAI services with the default configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddGovernAI(this IServiceCollection services)
        => services.AddGovernAI(_ => { });

    /// <summary>
    /// Registers GovernAI services with the specified configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">A delegate to configure <see cref="GovernAIOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddGovernAI(
        this IServiceCollection services,
        Action<GovernAIOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new GovernAIOptions();
        configure(options);

        // Prevent raw PII capture from being enabled in production environments (VULN-04).
        // Raw prompts and responses may contain PII, PHI, or trade secrets; storing them
        // in production violates privacy regulations and creates data-breach risk.
        if ((options.AllowRawPromptCapture || options.AllowRawResponseCapture)
            && (string.Equals(options.EnvironmentName, "Production", StringComparison.OrdinalIgnoreCase)
                || string.Equals(options.EnvironmentName, "Staging", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "AllowRawPromptCapture and AllowRawResponseCapture must not be enabled in " +
                "Production or Staging environments. Set these flags only for local development.");
        }

        // Enforce minimum HMAC key length: NIST SP 800-107 recommends a key length at least
        // equal to the hash output length (32 bytes for SHA-256) to maintain HMAC security.
        // 16 bytes is our enforced minimum; shorter keys reduce security margin.
        if (options.PromptHashKey is { Length: > 0 } hashKey && hashKey.Length < 16)
        {
            throw new ArgumentException(
                "PromptHashKey must be at least 16 bytes. " +
                "Recommended: 32 cryptographically random bytes stored in a secrets manager.",
                nameof(configure));
        }

        services.TryAddSingleton(options);

        // Clock
        services.TryAddSingleton<IGovernAIClock, SystemClock>();

        // Policy evaluator — default local evaluator using heuristic security scanning
        services.TryAddSingleton<IGovernAIPolicyEvaluator, DefaultLocalPolicyEvaluator>();

        // Redactor
        services.TryAddSingleton<IGovernAIRedactor, BasicPiiRedactor>();

        // Exporter — in-memory by default; application can replace with CompositeExporter
        services.TryAddSingleton<IGovernAIExporter>(sp =>
            new InMemoryExporter(sp.GetRequiredService<GovernAIOptions>().InMemoryExporterCapacity));

        // HTTP context infrastructure
        services.AddHttpContextAccessor();

        // Tenant / user resolvers
        services.TryAddSingleton<IGovernAITenantResolver, HttpContextTenantResolver>();
        services.TryAddSingleton<IGovernAIUserResolver, HttpContextUserResolver>();

        // Tracker (implementation) and Runtime (public entry point)
        services.TryAddSingleton<GovernAITracker>();
        services.TryAddSingleton<GovernAIRuntime>();

        return services;
    }
}
