# Phase 6 - Documentation

## Goal

Complete developer documentation for GovernAI.

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
/copilot/phase-6-docs.md
```

---

## Scope

Documentation only.

Do not change runtime behavior unless required to fix incorrect docs.

---

## Required Documentation Files

Ensure these are complete:

```text
README.md
docs/architecture.md
docs/coding-guidelines.md
docs/security-guidelines.md
docs/event-schema.md
docs/roadmap.md
```

Optionally add:

```text
docs/getting-started.md
docs/package-structure.md
docs/extensibility.md
```

---

## README Requirements

README must include:

```text
Project name
Project purpose
What GovernAI is
What GovernAI is not
Package list
Installation commands
Quick start
Basic usage example
ASP.NET Core usage example
Security defaults
Privacy defaults
Roadmap summary
Contribution guidance
License
```

---

## Installation Examples

Show:

```bash
dotnet add package GovernAI.Abstractions
dotnet add package GovernAI.Core
dotnet add package GovernAI.Security
dotnet add package GovernAI.AspNetCore
```

---

## Quick Start Example

Include minimal usage:

```csharp
builder.Services.AddGovernAI(options =>
{
    options.ApplicationName = "Enterprise.Api";
    options.EnvironmentName = builder.Environment.EnvironmentName;
});

app.UseGovernAI();
```

---

## Security Documentation

Clearly state:

- GovernAI does not fully prevent prompt injection.
- GovernAI provides governance assistance.
- Raw prompts are not stored by default.
- Raw responses are not stored by default.
- Sensitive data redaction is heuristic-based.

---

## Architecture Documentation

Must explain:

- local-first MVP
- future Collector
- future Policy Server
- future OpenTelemetry
- no provider lock-in
- no cloud lock-in

---

## Acceptance Criteria

- Docs are clear.
- Docs match implementation.
- No false claims.
- Examples compile or are clearly marked as illustrative.