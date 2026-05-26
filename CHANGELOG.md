# Changelog

All notable changes to the GovernAI .NET SDK are documented here.

This project follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0-alpha] — 2026-05-26

### Added

#### GovernAI.Abstractions
- Core interfaces: `IGovernAIExporter`, `IGovernAIPolicyEvaluator`, `IGovernAIRedactor`, `IGovernAITenantResolver`, `IGovernAIUserResolver`, `IGovernAIClock`
- Models: `GovernAIContext`, `GovernAIEvent`, `GovernAIPolicyDecision`
- Enums: `GovernAIRiskLevel`, `GovernAIPolicyDecisionType`
- `GovernAIDeniedException` thrown on policy denial

#### GovernAI.Core
- `GovernAITracker` — main entry point; wraps AI operations with full audit lifecycle
- `GovernAIRuntime` — high-level convenience façade over `GovernAITracker`
- `GovernAIOptions` — typed options for all SDK behaviour
- `PromptHasher` / `ResponseHasher` — SHA-256 (unkeyed) and HMAC-SHA256 (keyed) prompt/response fingerprinting
- `InMemoryExporter` — thread-safe bounded ring-buffer exporter for development and testing
- `FileExporter` — append-only JSON Lines file exporter; absolute-path enforced; implements `IDisposable`
- `CompositeExporter` — fan-out to multiple exporters with per-child failure logging
- `ConsoleExporter` — plain-text development exporter
- `NoOpExporter` — silent exporter for testing / opt-out
- `SystemClock` — production `IGovernAIClock` implementation
- `GovernAIJsonContext` — AOT-compatible source-generated JSON serialisation context

#### GovernAI.Security
- `BasicPiiRedactor` — regex-based redaction of emails, phone numbers, credit cards, SSNs, IP addresses
- `PromptInjectionHeuristicScanner` — heuristic detection of common prompt injection patterns
- `SensitiveDataScanner` — detects sensitive keywords and patterns in prompts/responses
- `RiskScoreCalculator` — combines multiple scan results into a composite risk score
- `DefaultLocalPolicyEvaluator` — in-process policy engine; Allow / Review / Deny decisions based on risk score

#### GovernAI.AspNetCore
- `UseGovernAI()` — registers the audit middleware pipeline
- `AddGovernAI(Action<GovernAIOptions>)` — DI registration with production safety guards
- `GovernAIMiddleware` — extracts and sanitises tenant ID, user ID, and correlation ID from HTTP context
- `HttpContextTenantResolver` — resolves tenant from JWT claims (priority) then `X-Tenant-Id` header
- `ClaimsTenantResolver` — claims-only tenant resolution
- `HttpContextUserResolver` — resolves authenticated user from `ClaimsPrincipal`
- `HeaderTenantResolver` — header-only tenant resolution

#### Samples
- `MinimalApi.Basic` — basic single-tenant tracking with in-memory events
- `MinimalApi.MultiTenant` — multi-tenant tracking with claim-based resolution
- `MinimalApi.PolicyDemo` — full Allow / Review / Deny policy demo with PII redaction

### Security
- Raw prompt and response capture disabled by default (`AllowRawPromptCapture = false`, `AllowRawResponseCapture = false`)
- Production/Staging guard prevents enabling raw capture in sensitive environments
- HMAC-SHA256 keyed hashing enforces minimum 16-byte key length
- Tenant ID resolved from authenticated claims before falling back to header (prevents tenant spoofing)
- All label fields sanitised to strip control characters and cap length (log injection prevention)
- Correlation ID sanitised in middleware (control characters stripped, capped at 128 chars)
- `FileExporter` rejects relative paths to prevent path traversal
- Input to all scanners and redactors capped at 64 KB to prevent ReDoS

### Performance
- `HMACSHA256.HashData()` static method used for HMAC computation (avoids per-call heap allocation)
- `SanitizeLabel` uses `stackalloc` for control-character stripping (no heap allocation for short strings)
- `InMemoryExporter` uses `ConcurrentQueue<T>` + lock-guarded trim for TOCTOU-safe capacity enforcement
- `FileExporter` uses `SemaphoreSlim(1,1)` for async-safe sequential writes

### Target Frameworks
- `net8.0`, `net10.0`
- AOT-compatible (`IsAotCompatible=true`, `IsTrimmable=true`)

---

## [Unreleased]

- GovernAI Collector HTTP exporter
- OpenTelemetry / GenAI semantic conventions integration
- Remote policy server support
- GovernAI Dashboard

---

[0.1.0-alpha]: https://github.com/DNVerma88/governai-dotnet/releases/tag/v0.1.0-alpha
[Unreleased]: https://github.com/DNVerma88/governai-dotnet/compare/v0.1.0-alpha...HEAD
