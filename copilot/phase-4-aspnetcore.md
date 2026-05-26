# Phase 4 - ASP.NET Core Integration

## Goal

Add ASP.NET Core integration for GovernAI.

This phase must provide dependency injection registration, middleware, correlation handling, tenant resolution, and user resolution.

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
/copilot/phase-4-aspnetcore.md
```

---

## Scope

Implement only `GovernAI.AspNetCore`.

Do not implement:

- OpenTelemetry
- Collector
- Remote Policy Server
- Dashboard
- Database storage

---

## Required Components

Create these in `GovernAI.AspNetCore`:

```text
GovernAIServiceCollectionExtensions
GovernAIApplicationBuilderExtensions
GovernAIMiddleware
HttpContextTenantResolver
HttpContextUserResolver
HeaderTenantResolver
ClaimsTenantResolver
GovernAIEndpointExtensions
```

---

## Dependency Injection

Support this usage:

```csharp
builder.Services.AddGovernAI(options =>
{
    options.ApplicationName = "Enterprise.Api";
    options.EnvironmentName = builder.Environment.EnvironmentName;
});
```

`AddGovernAI()` must register:

```text
GovernAIOptions
IGovernAIClock
IGovernAIPolicyEvaluator
IGovernAIExporter
IGovernAIRedactor
IGovernAITenantResolver
IGovernAIUserResolver
GovernAIRuntime/GovernAITracker
```

Use safe defaults.

---

## Middleware

Support this usage:

```csharp
app.UseGovernAI();
```

Middleware must:

- create correlation ID if missing
- reuse `X-Correlation-Id` if present
- add correlation ID to response header
- avoid reading request body
- avoid logging request body
- not block normal request execution

---

## Tenant Resolution

Support tenant resolution from:

```text
X-Tenant-Id header
tenant_id claim
tid claim
custom resolver
```

Priority:

```text
custom resolver
header
claims
empty
```

---

## User Resolution

Support user resolution from:

```text
sub claim
nameidentifier claim
email claim
custom resolver
```

Priority:

```text
custom resolver
claims
empty
```

---

## Endpoint Extensions

Provide simple endpoint extension hooks if needed.

Do not over-engineer.

---

## Security Rules

- Do not read request body.
- Do not store secrets.
- Do not log headers wholesale.
- Do not log authorization header.
- Do not log cookies.
- Do not fail request pipeline due to GovernAI internal errors by default.

---

## Tests

Add tests for:

- service registration
- middleware execution
- correlation ID generation
- existing correlation ID reuse
- tenant from header
- tenant from claims
- user from claims
- middleware does not read body

---

## Acceptance Criteria

- ASP.NET Core sample compiles.
- `AddGovernAI()` works.
- `UseGovernAI()` works.
- Correlation ID behavior works.
- Tenant/user resolution works.
- Tests pass.