using System.Security.Claims;
using GovernAI.Abstractions;
using GovernAI.AspNetCore;
using GovernAI.Core;
using GovernAI.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GovernAI.AspNetCore.Tests;

public sealed class ServiceRegistrationTests
{
    private static IServiceProvider BuildServiceProvider(Action<GovernAIOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddGovernAI(configure ?? (_ => { }));
        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddGovernAI_RegistersGovernAIOptions()
    {
        var sp = BuildServiceProvider(opts => opts.ApplicationName = "TestApp");
        var options = sp.GetRequiredService<GovernAIOptions>();
        Assert.Equal("TestApp", options.ApplicationName);
    }

    [Fact]
    public void AddGovernAI_RegistersIGovernAIClock()
    {
        var sp = BuildServiceProvider();
        var clock = sp.GetRequiredService<IGovernAIClock>();
        Assert.NotNull(clock);
    }

    [Fact]
    public void AddGovernAI_RegistersIGovernAIPolicyEvaluator()
    {
        var sp = BuildServiceProvider();
        var evaluator = sp.GetRequiredService<IGovernAIPolicyEvaluator>();
        Assert.IsType<DefaultLocalPolicyEvaluator>(evaluator);
    }

    [Fact]
    public void AddGovernAI_RegistersIGovernAIRedactor()
    {
        var sp = BuildServiceProvider();
        var redactor = sp.GetRequiredService<IGovernAIRedactor>();
        Assert.IsType<BasicPiiRedactor>(redactor);
    }

    [Fact]
    public void AddGovernAI_RegistersIGovernAIExporter()
    {
        var sp = BuildServiceProvider();
        var exporter = sp.GetRequiredService<IGovernAIExporter>();
        Assert.IsType<InMemoryExporter>(exporter);
    }

    [Fact]
    public void AddGovernAI_RegistersGovernAITracker()
    {
        var sp = BuildServiceProvider();
        var tracker = sp.GetRequiredService<GovernAITracker>();
        Assert.NotNull(tracker);
    }

    [Fact]
    public void AddGovernAI_RegistersGovernAIRuntime()
    {
        var sp = BuildServiceProvider();
        var runtime = sp.GetRequiredService<GovernAIRuntime>();
        Assert.NotNull(runtime);
    }

    [Fact]
    public void AddGovernAI_RegistersIGovernAITenantResolver()
    {
        var sp = BuildServiceProvider();
        var resolver = sp.GetRequiredService<IGovernAITenantResolver>();
        Assert.IsType<HttpContextTenantResolver>(resolver);
    }

    [Fact]
    public void AddGovernAI_RegistersIGovernAIUserResolver()
    {
        var sp = BuildServiceProvider();
        var resolver = sp.GetRequiredService<IGovernAIUserResolver>();
        Assert.IsType<HttpContextUserResolver>(resolver);
    }

    [Fact]
    public void AddGovernAI_PromptHashKeyTooShort_ThrowsArgumentException()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var ex = Assert.Throws<ArgumentException>(() =>
            services.AddGovernAI(opts =>
            {
                opts.PromptHashKey = new byte[8]; // less than 16 bytes minimum
            }));

        Assert.Contains("16 bytes", ex.Message);
    }

    [Fact]
    public void AddGovernAI_PromptHashKeyExactly16Bytes_DoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        // Should succeed — 16 bytes is the enforced minimum
        services.AddGovernAI(opts =>
        {
            opts.PromptHashKey = new byte[16];
        });
    }

    [Fact]
    public void AddGovernAI_PromptHashKeyNull_DoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        // Null key is allowed — hashing falls back to plain SHA-256
        services.AddGovernAI(opts =>
        {
            opts.PromptHashKey = null;
        });
    }
}
