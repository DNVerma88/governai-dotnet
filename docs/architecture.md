# GovernAI Architecture

## Purpose

GovernAI is a dependency-light, local-first .NET SDK for AI governance, auditability, policy enforcement, redaction, risk scoring, and tenant-aware AI execution tracking.

The first version must run fully in-process without external infrastructure, while keeping the architecture ready for future expansion into:

- GovernAI Collector
- Remote Policy Server
- OpenTelemetry integration
- Dashboard
- Storage
- MCP security
- Cross-language SDKs

---

## Repository Name

```text
governai-dotnet
```

---

## Architecture Style

GovernAI follows a modular SDK architecture.

Each package must have a clear responsibility and must be independently reusable.

```text
.NET Application
   |
   |-- GovernAI.AspNetCore
   |      - ASP.NET Core middleware
   |      - dependency injection registration
   |      - correlation ID handling
   |      - tenant/user resolution
   |
   |-- GovernAI.Core
   |      - runtime tracking
   |      - prompt/response hashing
   |      - event creation
   |      - local exporters
   |      - local policy execution
   |
   |-- GovernAI.Security
   |      - PII redaction
   |      - sensitive data detection
   |      - prompt risk scanning
   |      - local risk scoring
   |
   |-- GovernAI.Abstractions
          - interfaces
          - models
          - enums
          - shared contracts
```

---

## Package Structure

```text
src/
  GovernAI.Abstractions/
  GovernAI.Core/
  GovernAI.Security/
  GovernAI.AspNetCore/
```

Each project must produce a separate NuGet package.

```text
GovernAI.Abstractions
GovernAI.Core
GovernAI.Security
GovernAI.AspNetCore
```

---

## Package Dependency Rules

```text
GovernAI.Abstractions
  - No project dependency
  - No external package dependency

GovernAI.Core
  - Depends on GovernAI.Abstractions
  - No external package dependency

GovernAI.Security
  - Depends on GovernAI.Abstractions
  - Depends on GovernAI.Core
  - No external package dependency

GovernAI.AspNetCore
  - Depends on GovernAI.Abstractions
  - Depends on GovernAI.Core
  - Depends on GovernAI.Security
  - May use ASP.NET Core shared framework references only
```

---

## MVP Runtime Architecture

```text
Application Code
   |
   | calls TrackAsync()
   |
GovernAI Runtime
   |
   |-- Resolve context
   |-- Evaluate local policy
   |-- Execute AI operation if allowed
   |-- Hash prompt/response
   |-- Redact sensitive values where configured
   |-- Build GovernAIEvent
   |-- Export event
   |
Exporter Interface
   |-- InMemoryExporter
   |-- ConsoleExporter
   |-- FileExporter
   |-- CompositeExporter
```

---

## Future Collector Architecture

The SDK must be designed so an HTTP exporter can be added later without changing core runtime contracts.

```text
.NET Application
   |
GovernAI SDK
   |
IGovernAIExporter
   |
GovernAI.Exporter.Http
   |
GovernAI Collector
   |
Storage / Dashboard / SIEM / Azure Monitor / Datadog / Elastic
```

Do not implement the Collector in MVP.

---

## Future Remote Policy Server Architecture

The SDK must be designed so local policy evaluation can later be replaced or supplemented by a remote policy server.

```text
.NET Application
   |
GovernAI SDK
   |
IGovernAIPolicyEvaluator
   |
GovernAI.Policy.Client
   |
GovernAI Policy Server
   |
Allow / Review / Deny
```

Do not implement the Policy Server in MVP.

---

## Future OpenTelemetry Architecture

The SDK must be designed so events can later be mapped to OpenTelemetry GenAI semantic conventions.

```text
GovernAIEvent
   |
GovernAI.OpenTelemetry
   |
OpenTelemetry Traces / Metrics / Logs
   |
Application Insights / Azure Monitor / Grafana / Datadog / Elastic
```

Do not implement OpenTelemetry in MVP.

---

## Core Extension Points

GovernAI must expose small focused interfaces.

```csharp
IGovernAIExporter
IGovernAIPolicyEvaluator
IGovernAIRedactor
IGovernAITenantResolver
IGovernAIUserResolver
IGovernAIClock
```

These interfaces are required so the SDK can be extended without modifying core runtime logic.

---

## Design Principles

The architecture must follow:

- SOLID
- KISS
- DRY
- YAGNI
- Composition over inheritance
- Interface-based extensibility
- Secure by default
- Privacy by default
- Provider-neutral design
- Cloud-neutral design
- Tenant-aware design

---

## Design Patterns

Use these patterns only where they add clear value:

### Strategy Pattern

Use for:

- policy evaluation
- redaction
- tenant resolution
- user resolution

### Composite Pattern

Use for:

- multiple exporters

### Null Object Pattern

Use for:

- no-op policy evaluator
- no-op exporter

### Adapter Pattern

Use later for:

- OpenTelemetry
- Collector
- Policy Server

### Middleware Pattern

Use for:

- ASP.NET Core integration

### Options Pattern

Use for:

- SDK configuration

---

## Security Architecture

GovernAI must be secure by default.

Default behavior:

- Do not store raw prompt.
- Do not store raw response.
- Hash prompt using SHA-256.
- Hash response using SHA-256.
- Avoid logging request bodies.
- Avoid collecting secrets.
- Support redaction.
- Support risk scoring.
- Support policy decisions.

---

## Multi-Tenant Architecture

GovernAI must support tenant-aware applications from the beginning.

Tenant ID can come from:

- explicit context
- HTTP header
- claims
- custom resolver

The SDK must not assume a specific tenant model.

---

## Provider-Neutral Architecture

GovernAI must not depend on:

- OpenAI
- Azure OpenAI
- Anthropic
- Gemini
- AWS Bedrock
- Semantic Kernel
- LangChain

Applications can use any AI provider and wrap the call with GovernAI tracking.

---

## Cloud-Neutral Architecture

GovernAI must not depend on:

- Azure SDK
- AWS SDK
- GCP SDK

Cloud integrations must be added later as optional packages only.

---

## Non-Functional Requirements

GovernAI must be:

- thread-safe
- async-first
- testable
- extensible
- lightweight
- AOT-friendly
- trimming-friendly
- dependency-light
- high-throughput API friendly

---

## MVP Scope

Implement only:

```text
GovernAI.Abstractions
GovernAI.Core
GovernAI.Security
GovernAI.AspNetCore
```

Do not implement:

```text
GovernAI.OpenTelemetry
GovernAI.Collector
GovernAI.PolicyServer
GovernAI.Dashboard
GovernAI.Storage
GovernAI.MCP
```