# GovernAI Coding Guidelines

# General Principles

The GovernAI SDK must prioritize:

- simplicity
- maintainability
- extensibility
- performance
- security
- readability
- testability

---

# SOLID Principles

## Single Responsibility Principle

Each class should have one responsibility.

Examples:

- PromptHasher => hashing only
- ResponseHasher => hashing only
- BasicPiiRedactor => redaction only
- CompositeExporter => exporter orchestration only

---

## Open Closed Principle

New exporters, policy evaluators, and redactors should be extendable without modifying existing runtime code.

---

## Liskov Substitution Principle

All implementations must safely substitute their interfaces.

---

## Interface Segregation Principle

Keep interfaces focused and small.

Avoid large multi-purpose interfaces.

---

## Dependency Inversion Principle

Depend on abstractions rather than concrete implementations.

---

# KISS

Keep implementation simple.

Avoid:

- deep inheritance
- unnecessary abstraction
- speculative architecture
- overly generic frameworks

---

# DRY

Avoid duplication across:

- event creation
- hashing
- exporter orchestration
- policy logic
- redaction logic

---

# YAGNI

Do not implement features before they are required.

Examples:

- dashboards
- distributed storage
- remote collectors
- OpenTelemetry exporters
- distributed policy engines

must not be added until explicitly requested.

---

# Performance Guidelines

The SDK must:

- minimize allocations
- avoid reflection
- avoid runtime assembly scanning
- avoid blocking calls
- use async APIs
- support cancellation tokens
- support trimming
- support Native AOT

---

# Async Guidelines

Prefer:

- async/await
- ValueTask where appropriate
- CancellationToken support

Avoid synchronous I/O.

---

# Error Handling

- SDK should not crash host applications by default.
- Exporter failure should not break request execution by default.
- Policy denial should be explicit.
- Failed AI operations must still generate events.

---

# Thread Safety

- All shared services must be thread-safe.
- Avoid mutable shared state.
- Prefer immutable records.

---

# Naming Guidelines

Use clear names.

Examples:

Good:

- PromptHasher
- CompositeExporter
- DefaultLocalPolicyEvaluator

Bad:

- Utils
- Helper
- Manager
- Processor

---

# Architecture Guidelines

Prefer:

- composition over inheritance
- interface-driven architecture
- explicit dependencies
- constructor injection

Avoid:

- service locator
- static mutable state
- hidden side effects

---

# Testing Guidelines

All features must have unit tests.

Tests should cover:

- success scenarios
- failure scenarios
- edge cases
- concurrency
- redaction
- hashing
- policy evaluation
- exporter failures