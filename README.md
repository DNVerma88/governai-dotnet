# GovernAI .NET SDK

[![Build](https://github.com/DNVerma88/governai-dotnet/actions/workflows/build.yml/badge.svg)](https://github.com/DNVerma88/governai-dotnet/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/GovernAI.Abstractions?label=GovernAI.Abstractions)](https://www.nuget.org/packages/GovernAI.Abstractions/)
[![NuGet](https://img.shields.io/nuget/v/GovernAI.Core?label=GovernAI.Core)](https://www.nuget.org/packages/GovernAI.Core/)
[![NuGet](https://img.shields.io/nuget/v/GovernAI.Security?label=GovernAI.Security)](https://www.nuget.org/packages/GovernAI.Security/)
[![NuGet](https://img.shields.io/nuget/v/GovernAI.AspNetCore?label=GovernAI.AspNetCore)](https://www.nuget.org/packages/GovernAI.AspNetCore/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

GovernAI is a dependency-light, local-first .NET SDK for AI governance, runtime auditing, policy enforcement, prompt/response hashing, sensitive data redaction, risk analysis, and tenant-aware AI execution tracking.

---

## Packages

| Package | Description |
|---------|-------------|
| `GovernAI.Abstractions` | Core interfaces, models, and enums |
| `GovernAI.Core` | Runtime tracking, hashing, and event export |
| `GovernAI.Security` | PII redaction, risk scoring, prompt injection detection |
| `GovernAI.AspNetCore` | ASP.NET Core middleware and DI integration |

---

## Quick Start

### Install

```bash
dotnet add package GovernAI.Abstractions
dotnet add package GovernAI.Core
dotnet add package GovernAI.Security
dotnet add package GovernAI.AspNetCore
```

### Register Services (Program.cs)

```csharp
builder.Services.AddGovernAI(options =>
{
    options.ApplicationName = "MyApp";
    options.EnvironmentName = builder.Environment.EnvironmentName;
});

app.UseGovernAI();
```

### Track an AI Call

```csharp
app.MapPost("/api/chat", async (ChatRequest req, GovernAITracker tracker) =>
{
    var context = new GovernAIContext
    {
        OperationName = "chat",
        ModelProvider = "openai",
        ModelName = "gpt-4o",
        Prompt = req.Prompt
    };

    try
    {
        var result = await tracker.TrackAsync(context,
            async ct => await myAiClient.CompleteAsync(req.Prompt, ct));

        return Results.Ok(result);
    }
    catch (GovernAIDeniedException ex)
    {
        return Results.Problem(
            detail: ex.PolicyDecision.Reason,
            statusCode: StatusCodes.Status403Forbidden,
            title: "Request denied by policy");
    }
});
```

---

## Samples

| Sample | Description |
|--------|-------------|
| [`MinimalApi.Basic`](samples/MinimalApi.Basic/) | Basic setup with tracking and in-memory events |
| [`MinimalApi.MultiTenant`](samples/MinimalApi.MultiTenant/) | Multi-tenant via `X-Tenant-Id` header or claims |
| [`MinimalApi.PolicyDemo`](samples/MinimalApi.PolicyDemo/) | Allow / Review / Deny policy outcomes |

---

## Target Frameworks

All packages target:

- `net8.0`
- `net10.0`

---

## Design Goals

- **Dependency-light** — no external NuGet dependencies in SDK packages
- **Local-first** — runs fully in-process without external infrastructure
- **Provider-neutral** — works with any AI provider (OpenAI, Anthropic, Gemini, local models, etc.)
- **Cloud-neutral** — no coupling to Azure, AWS, or GCP
- **Secure by default** — raw prompts and responses are not stored by default
- **Privacy by default** — SHA-256 hashing used instead of raw content storage
- **AOT-compatible** — supports Native AOT and trimming
- **Multi-tenant-ready** — tenant-aware from the foundation
- **OpenTelemetry-ready** — schema maps to GenAI semantic conventions (future)
- **Collector-ready** — events designed for future HTTP export (future)

---

## Architecture

```
.NET Application
   |
   |-- GovernAI.AspNetCore    (middleware, DI, HTTP context resolvers)
   |-- GovernAI.Core          (runtime, hashing, exporters, policy)
   |-- GovernAI.Security      (redaction, risk scoring, injection detection)
   |-- GovernAI.Abstractions  (interfaces, models, enums)
```

---

## Security

GovernAI aligns with [OWASP LLM Top 10](https://owasp.org/www-project-top-10-for-large-language-model-applications/) guidance and focuses on:

- Prompt Injection detection
- Sensitive Information Disclosure prevention
- Insecure Output Handling
- Excessive Agency governance
- Supply Chain Vulnerability awareness

GovernAI provides **heuristic-based protection and governance assistance only**. It does not guarantee full prompt injection or jailbreak prevention.

---

## Repository Structure

```
governai-dotnet/
├── Directory.Build.props
├── GovernAI.slnx
├── src/
│   ├── GovernAI.Abstractions/
│   ├── GovernAI.Core/
│   ├── GovernAI.Security/
│   └── GovernAI.AspNetCore/
├── tests/
│   ├── GovernAI.Abstractions.Tests/
│   ├── GovernAI.Core.Tests/
│   ├── GovernAI.Security.Tests/
│   └── GovernAI.AspNetCore.Tests/
└── samples/
│   ├── MinimalApi.Basic/
│   ├── MinimalApi.MultiTenant/
│   └── MinimalApi.PolicyDemo/
├── docs/
└── copilot/
```

---

## What GovernAI Is Not

- **Not a firewall** — heuristic detection only; does not guarantee prevention of prompt injection or jailbreaks
- **Not an AI provider** — no built-in connection to OpenAI, Azure, Anthropic, or any LLM
- **Not a data store** — events are stored in-memory by default; raw prompts/responses are never stored
- **Not a compliance solution** — governance assistance only; consult your legal/security team for compliance requirements
- **Not cloud infrastructure** — runs fully in-process, no external services required in MVP

---

## Security Defaults

| Behavior | Default |
|----------|---------|
| Raw prompt stored | ❌ No — SHA-256 hash only |
| Raw response stored | ❌ No — SHA-256 hash only |
| Prompt hashing | ✅ Enabled |
| Response hashing | ✅ Enabled |
| Exporter error breaks request | ❌ No — swallowed by default |
| Policy denial throws exception | ✅ Yes — `GovernAIDeniedException` |

GovernAI does **not** fully prevent prompt injection. It provides heuristic-based governance assistance.

---

## Privacy Defaults

- Raw prompts are **never stored** unless `AllowRawPromptCapture = true`
- Raw responses are **never stored** unless `AllowRawResponseCapture = true`
- Sensitive data redaction is **heuristic-based** — not a guarantee
- `GovernAIEvent` stores only SHA-256 hashes of prompt/response content

---

## Roadmap Summary

| Phase | Status | Description |
|-------|--------|-------------|
| 1 — Foundation | ✅ Complete | Abstractions, interfaces, models |
| 2 — Core Runtime | ✅ Complete | Tracker, exporters, hashing |
| 3 — Security | ✅ Complete | PII redaction, injection detection, policy |
| 4 — ASP.NET Core | ✅ Complete | Middleware, DI, tenant/user resolution |
| 5 — Samples | ✅ Complete | Three minimal API samples |
| 6 — Documentation | ✅ Complete | Full developer docs |
| 7 — CI/CD | ✅ Complete | GitHub Actions build/test/pack |
| Future | Planned | GovernAI Collector, Remote Policy Server, OpenTelemetry, Dashboard |

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for full guidance.

Quick start:

```bash
git clone https://github.com/DNVerma88/governai-dotnet
cd governai-dotnet
dotnet build
dotnet test
```

- All public APIs must have XML documentation
- No external NuGet packages in SDK source projects
- All changes must include tests
- Follow the [coding guidelines](docs/coding-guidelines.md) and [security guidelines](docs/security-guidelines.md)

---

## License

[MIT](LICENSE)
