# GovernAI Roadmap

## Product Direction

GovernAI will start as a dependency-light, local-first .NET SDK.

The long-term goal is to evolve into a broader AI governance ecosystem supporting:

- .NET
- Java
- Python
- PHP
- JavaScript/TypeScript
- Collector
- Policy Server
- Dashboard
- OpenTelemetry
- MCP security

---

# Phase 1 - Foundation

## Goal

Create the base repository, solution, packages, contracts, models, and documentation placeholders.

## Packages

```text
GovernAI.Abstractions
GovernAI.Core
GovernAI.Security
GovernAI.AspNetCore
```

## Deliverables

- Solution file
- Source projects
- Test projects
- Directory.Build.props
- Common metadata
- Interfaces
- Models
- Enums
- README placeholder
- Documentation placeholders

## Success Criteria

- Solution builds.
- Tests run.
- No external package dependencies.
- All projects target net8.0 and net10.0.
- Public APIs have XML comments.

---

# Phase 2 - Core Runtime

## Goal

Implement local-first AI execution tracking.

## Deliverables

- GovernAIRuntime
- GovernAITracker
- GovernAIOptions
- PromptHasher
- ResponseHasher
- SystemClock
- NoOpPolicyEvaluator
- NoOpExporter
- CompositeExporter
- InMemoryExporter
- ConsoleExporter
- FileExporter

## Success Criteria

- Successful AI operation is tracked.
- Failed AI operation is tracked.
- Duration is captured.
- Prompt and response are hashed.
- Event is exported.
- Exporter failure does not break app by default.

---

# Phase 3 - Security

## Goal

Add basic security scanning, redaction, risk scoring, and local policy decisions.

## Deliverables

- BasicPiiRedactor
- SensitiveDataScanner
- PromptInjectionHeuristicScanner
- RiskScoreCalculator
- DefaultLocalPolicyEvaluator

## Success Criteria

- Common sensitive data is redacted.
- Prompt injection-like patterns are detected.
- Risk level is calculated.
- Local policy decision is returned.
- No false claim of full protection.

---

# Phase 4 - ASP.NET Core Integration

## Goal

Provide first-class ASP.NET Core integration.

## Deliverables

- AddGovernAI()
- UseGovernAI()
- GovernAIMiddleware
- HttpContextTenantResolver
- HttpContextUserResolver
- HeaderTenantResolver
- ClaimsTenantResolver
- GovernAIEndpointExtensions

## Success Criteria

- Minimal API sample works.
- Tenant can be resolved from header.
- User can be resolved from claims.
- Correlation ID is generated or reused.
- Middleware does not read request body.

---

# Phase 5 - Samples

## Goal

Provide working developer examples.

## Samples

```text
samples/MinimalApi.Basic
samples/MinimalApi.MultiTenant
samples/MinimalApi.PolicyDemo
```

## Success Criteria

- Samples compile.
- Samples run without real AI provider.
- Samples use fake AI calls.
- README explains how to run samples.

---

# Phase 6 - Documentation

## Goal

Create useful developer documentation.

## Deliverables

- README
- architecture.md
- coding-guidelines.md
- security-guidelines.md
- event-schema.md
- roadmap.md
- getting-started.md if needed

## Success Criteria

- Docs explain purpose clearly.
- Docs explain secure defaults.
- Docs explain package structure.
- Docs explain future extensibility.
- Docs avoid overclaiming security guarantees.

---

# Phase 7 - CI/CD

## Goal

Prepare the repository for OSS quality.

## Deliverables

- GitHub Actions workflow
- Restore
- Build
- Test
- Pack
- Pull request validation
- Main branch validation

## Success Criteria

- CI builds all projects.
- CI runs tests.
- CI creates NuGet packages.
- CI does not publish packages yet.

---

# Future Phase 8 - OpenTelemetry

## Goal

Add optional OpenTelemetry integration.

## Package

```text
GovernAI.OpenTelemetry
```

## Scope

- Map GovernAIEvent to OpenTelemetry GenAI conventions.
- Add traces.
- Add metrics.
- Keep package optional.

Do not include this in MVP.

---

# Future Phase 9 - Collector

## Goal

Create centralized event ingestion.

## Packages

```text
GovernAI.Exporter.Http
GovernAI.Collector
```

## Scope

- HTTP event exporter
- Collector API
- Event ingestion
- Future storage integration
- Future dashboard support

Do not include this in MVP.

---

# Future Phase 10 - Remote Policy Server

## Goal

Centralize policy decisions.

## Packages

```text
GovernAI.Policy.Client
GovernAI.PolicyServer
```

## Scope

- Remote policy evaluation
- Tenant-level policy rules
- Policy caching
- Fallback behavior
- Allow / Review / Deny decisions

Do not include this in MVP.

---

# Future Phase 11 - MCP Security

## Goal

Add MCP governance and security support.

## Packages

```text
GovernAI.MCP
GovernAI.MCP.Security
```

## Scope

- Tool authorization
- MCP audit trail
- Tool execution policies
- Secret protection
- Tenant-aware MCP access

Do not include this in MVP.

---

# Future Phase 12 - Cross-Language SDKs

## Goal

Expand GovernAI beyond .NET.

## Repositories

```text
governai-java
governai-python
governai-php
governai-js
```

## Rule

All SDKs must follow the same event schema.

---

# MVP Boundary

MVP includes only:

```text
GovernAI.Abstractions
GovernAI.Core
GovernAI.Security
GovernAI.AspNetCore
```

MVP excludes:

```text
GovernAI.OpenTelemetry
GovernAI.Collector
GovernAI.PolicyServer
GovernAI.Dashboard
GovernAI.Storage
GovernAI.MCP
```