# GovernAI Master Instructions

## Product Vision

GovernAI is a dependency-light, local-first, extensible .NET SDK for:

- AI governance
- AI runtime auditing
- AI policy enforcement
- Prompt/response hashing
- Sensitive data redaction
- Prompt risk analysis
- Tenant-aware AI execution tracking
- ASP.NET Core integration
- Future OpenTelemetry integration
- Future Collector integration
- Future Remote Policy Server integration

The SDK must initially run fully in-process without requiring external infrastructure.

The architecture must be designed so that future versions can support:

- GovernAI Collector
- GovernAI Remote Policy Server
- GovernAI Dashboard
- OpenTelemetry integration
- MCP security
- Cross-language SDKs

---

# Architecture Goals

The SDK architecture must be:

- Local-first
- Dependency-light
- Vendor-neutral
- Cloud-neutral
- Extensible
- AOT-friendly
- Trimming-friendly
- Thread-safe
- Async-first
- Multi-tenant-ready
- OpenTelemetry-ready
- Collector-ready
- Policy-server-ready

---

# Hard Requirements

## External Dependencies

The MVP must NOT add external NuGet package dependencies unless explicitly approved.

Do NOT use:

- Semantic Kernel
- LangChain
- OpenAI SDK
- Azure SDK
- AWS SDK
- Entity Framework
- Serilog
- Newtonsoft.Json
- MediatR
- Polly
- AutoMapper

Use only built-in .NET libraries unless explicitly approved.

---

# Target Frameworks

All packages must target:

```xml
<TargetFrameworks>net8.0;net10.0</TargetFrameworks>
```

All projects must enable:

```xml
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
<GenerateDocumentationFile>true</GenerateDocumentationFile>
<IsAotCompatible>true</IsAotCompatible>
<IsTrimmable>true</IsTrimmable>
<EnableTrimAnalyzer>true</EnableTrimAnalyzer>
<EnableAotAnalyzer>true</EnableAotAnalyzer>
```

---

# Design Principles

The entire SDK must follow these principles:

## SOLID

### Single Responsibility Principle

Each class must have one responsibility only.

Examples:

- PromptHasher => hashing only
- BasicPiiRedactor => redaction only
- CompositeExporter => exporter orchestration only

### Open Closed Principle

SDK must support extensibility through interfaces and composition without modifying existing code.

### Liskov Substitution Principle

All interface implementations must behave consistently and safely.

### Interface Segregation Principle

Keep interfaces small and focused.

### Dependency Inversion Principle

Core runtime must depend on abstractions rather than concrete implementations.

---

## KISS

Keep implementation simple.

Avoid:

- unnecessary abstraction
- over-engineering
- speculative features
- complex policy engines in MVP

---

## DRY

Avoid duplicated:

- hashing logic
- redaction logic
- policy evaluation logic
- event creation logic

---

## YAGNI

Do not implement features that are not immediately required.

Do not build:

- dashboards
- database storage
- distributed policy engines
- remote collectors
- OpenTelemetry exporters

until explicitly requested.

---

# Architecture Principles

## Composition Over Inheritance

Prefer composition and interfaces instead of deep inheritance hierarchies.

## Interface-Based Extensibility

All extension points must use interfaces.

Examples:

- IGovernAIExporter
- IGovernAIPolicyEvaluator
- IGovernAIRedactor
- IGovernAITenantResolver
- IGovernAIUserResolver

## Secure By Default

Default configuration must prioritize safety and privacy.

## Privacy By Default

Raw prompts and responses must NOT be stored by default.

## Provider Neutral

Do not couple to OpenAI, Azure OpenAI, Anthropic, or any specific provider.

## Cloud Neutral

Do not couple to Azure, AWS, or GCP.

## Tenant-Aware Design

Architecture must support multi-tenant applications from the beginning.

---

# Design Patterns To Use

Use patterns only where they provide real value.

## Strategy Pattern

Use for:

- policy evaluation
- redaction
- tenant resolution
- user resolution

## Factory Pattern

Use only where object creation becomes complex.

## Options Pattern

Use for SDK configuration.

## Composite Pattern

Use for multi-exporter support.

## Null Object Pattern

Use for:

- NoOpPolicyEvaluator
- NoOpExporter

## Adapter Pattern

Use for future:

- OpenTelemetry integration
- Collector integration
- Policy server integration

## Middleware Pattern

Use for ASP.NET Core integration.

---

# Security Guidelines

GovernAI must align with OWASP LLM security guidance.

## Key Risk Areas

Focus especially on:

- Prompt Injection
- Sensitive Information Disclosure
- Insecure Output Handling
- Excessive Agency
- Supply Chain Vulnerabilities
- Model Denial of Service
- Insecure Plugin/Tool Design

---

# Security Defaults

GovernAI must provide secure defaults.

## Prompt Handling

- Do not store raw prompt by default.
- Do not store raw response by default.
- Hash prompts using SHA-256.
- Hash responses using SHA-256.

## Redaction

Redact:

- email addresses
- phone numbers
- API keys
- bearer tokens
- JWT tokens
- connection strings
- password-like values

## Prompt Risk Detection

Detect risky prompt patterns such as:

- ignore previous instructions
- reveal system prompt
- bypass security
- disable policy
- print secrets
- exfiltrate data
- jailbreak
- hidden instructions

## Policy Decisions

Support:

- Allow
- Review
- Deny

Default behavior:

- Low risk => Allow
- Medium risk => Allow with warning
- High risk => Review
- Critical risk => Deny

---

# Performance Guidelines

The SDK must:

- minimize allocations
- avoid reflection
- avoid dynamic code generation
- avoid runtime assembly scanning
- avoid blocking calls
- use async APIs
- support cancellation tokens
- support AOT
- support trimming

---

# Testing Requirements

All features must have unit tests.

Tests must cover:

- success scenarios
- failure scenarios
- edge cases
- concurrency behavior
- hashing
- redaction
- policy evaluation
- exporter behavior

---

# Documentation Requirements

All public APIs must:

- have XML documentation
- have meaningful names
- follow consistent naming conventions

All phases must update:

- README
- docs
- samples

---

# Repository Structure

```text
governai-dotnet/
│
├── README.md
├── LICENSE
├── CONTRIBUTING.md
├── .gitignore
├── .editorconfig
├── Directory.Build.props
├── GovernAI.sln
│
├── docs/
├── copilot/
├── src/
├── tests/
└── samples/
```

---

# Implementation Rules For Copilot Agent

1. Implement one phase at a time.
2. Do not implement future phases prematurely.
3. Keep public APIs minimal and stable.
4. Keep architecture extensible.
5. Keep code easy to understand.
6. Keep secure defaults.
7. Do not add external dependencies.
8. Prioritize maintainability and clarity.
9. Ensure all projects build for net8.0 and net10.0.
10. Keep the SDK future-ready for Collector and Policy Server expansion.