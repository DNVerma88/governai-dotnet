# Phase 1 - Foundation

## Goal

Create the initial GovernAI .NET SDK repository foundation.

This phase must create the solution structure, source projects, test projects, shared build settings, core abstractions, models, and enums.

Do not implement runtime behavior in this phase.

---

## Files To Read Before Starting

Read these files first:

```text
/copilot/00-master-instructions.md
/docs/architecture.md
/docs/coding-guidelines.md
/docs/security-guidelines.md
/docs/event-schema.md
/docs/roadmap.md
```

---

## Scope

Implement only:

- Solution file
- Source project structure
- Test project structure
- Directory.Build.props
- Core interfaces
- Core models
- Core enums
- Basic README placeholder
- Basic documentation placeholders

---

## Repository Structure To Create

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
│   ├── architecture.md
│   ├── coding-guidelines.md
│   ├── security-guidelines.md
│   ├── event-schema.md
│   └── roadmap.md
│
├── copilot/
│   ├── 00-master-instructions.md
│   ├── phase-1-foundation.md
│   ├── phase-2-core-runtime.md
│   ├── phase-3-security.md
│   ├── phase-4-aspnetcore.md
│   ├── phase-5-samples.md
│   ├── phase-6-docs.md
│   └── phase-7-cicd.md
│
├── src/
│   ├── GovernAI.Abstractions/
│   ├── GovernAI.Core/
│   ├── GovernAI.Security/
│   └── GovernAI.AspNetCore/
│
├── tests/
│   ├── GovernAI.Abstractions.Tests/
│   ├── GovernAI.Core.Tests/
│   ├── GovernAI.Security.Tests/
│   └── GovernAI.AspNetCore.Tests/
│
└── samples/
    ├── MinimalApi.Basic/
    ├── MinimalApi.MultiTenant/
    └── MinimalApi.PolicyDemo/
```

---

## Project Requirements

Create these source projects:

```text
src/GovernAI.Abstractions/GovernAI.Abstractions.csproj
src/GovernAI.Core/GovernAI.Core.csproj
src/GovernAI.Security/GovernAI.Security.csproj
src/GovernAI.AspNetCore/GovernAI.AspNetCore.csproj
```

Create these test projects:

```text
tests/GovernAI.Abstractions.Tests/GovernAI.Abstractions.Tests.csproj
tests/GovernAI.Core.Tests/GovernAI.Core.Tests.csproj
tests/GovernAI.Security.Tests/GovernAI.Security.Tests.csproj
tests/GovernAI.AspNetCore.Tests/GovernAI.AspNetCore.Tests.csproj
```

---

## Target Frameworks

All projects must target:

```xml
<TargetFrameworks>net8.0;net10.0</TargetFrameworks>
```

---

## Build Settings

Create `Directory.Build.props` at repository root.

It must include:

```xml
<Project>
  <PropertyGroup>
    <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <IsAotCompatible>true</IsAotCompatible>
    <IsTrimmable>true</IsTrimmable>
    <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
    <EnableAotAnalyzer>true</EnableAotAnalyzer>
    <Authors>GovernAI Contributors</Authors>
    <Company>GovernAI</Company>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <RepositoryType>git</RepositoryType>
  </PropertyGroup>
</Project>
```

---

## Dependency Rules

Do not add external NuGet packages.

Allowed:

- Project references between GovernAI projects.
- ASP.NET Core shared framework reference in `GovernAI.AspNetCore` only if required.

Not allowed:

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

---

## Package Dependency Rules

```text
GovernAI.Abstractions
  - No dependency

GovernAI.Core
  - Depends on GovernAI.Abstractions

GovernAI.Security
  - Depends on GovernAI.Abstractions
  - Depends on GovernAI.Core

GovernAI.AspNetCore
  - Depends on GovernAI.Abstractions
  - Depends on GovernAI.Core
  - Depends on GovernAI.Security
```

---

## Required Interfaces

Create these interfaces in `GovernAI.Abstractions`.

### `IGovernAIExporter`

```csharp
public interface IGovernAIExporter
{
    ValueTask ExportAsync(GovernAIEvent aiEvent, CancellationToken cancellationToken = default);
}
```

### `IGovernAIPolicyEvaluator`

```csharp
public interface IGovernAIPolicyEvaluator
{
    ValueTask<GovernAIPolicyDecision> EvaluateAsync(
        GovernAIContext context,
        CancellationToken cancellationToken = default);
}
```

### `IGovernAIRedactor`

```csharp
public interface IGovernAIRedactor
{
    string Redact(string? input);
}
```

### `IGovernAITenantResolver`

```csharp
public interface IGovernAITenantResolver
{
    ValueTask<string?> ResolveTenantIdAsync(CancellationToken cancellationToken = default);
}
```

### `IGovernAIUserResolver`

```csharp
public interface IGovernAIUserResolver
{
    ValueTask<string?> ResolveUserIdAsync(CancellationToken cancellationToken = default);
}
```

### `IGovernAIClock`

```csharp
public interface IGovernAIClock
{
    DateTimeOffset UtcNow { get; }
}
```

---

## Required Models

Create these records in `GovernAI.Abstractions`.

### `GovernAIContext`

Must include:

```text
TraceId
CorrelationId
ApplicationName
EnvironmentName
TenantId
UserId
AgentName
OperationName
ModelProvider
ModelName
Prompt
Response
InputTokens
OutputTokens
Metadata
```

### `GovernAIEvent`

Must include:

```text
EventId
TraceId
CorrelationId
ApplicationName
EnvironmentName
TenantId
UserId
AgentName
OperationName
ModelProvider
ModelName
PromptHash
ResponseHash
InputTokens
OutputTokens
TotalTokens
RiskScore
RiskLevel
RiskCategory
PolicyDecision
PolicyReason
DurationMs
Success
ErrorCode
ErrorMessage
TimestampUtc
Metadata
```

### `GovernAIPolicyDecision`

Must include:

```text
Decision
Reason
RiskScore
RiskLevel
RiskCategory
Metadata
```

### `GovernAIRiskResult`

Must include:

```text
RiskScore
RiskLevel
RiskCategory
Reason
MatchedPatterns
```

---

## Required Enums

Create:

```text
GovernAIPolicyDecisionType
GovernAIRiskLevel
```

### `GovernAIPolicyDecisionType`

Values:

```text
Allow
Review
Deny
```

### `GovernAIRiskLevel`

Values:

```text
None
Low
Medium
High
Critical
```

---

## XML Documentation

All public APIs must have XML comments.

---

## Tests

Create basic tests to verify:

- models can be created
- enum values exist
- interfaces compile through fake implementations

Do not add external test packages unless already available in template.

If test framework requires packages, use standard .NET test template packages only.

---

## Acceptance Criteria

- Solution builds.
- All projects target `net8.0` and `net10.0`.
- No external runtime package dependencies are added.
- Public APIs have XML comments.
- Basic tests pass.
- No runtime logic implemented yet.