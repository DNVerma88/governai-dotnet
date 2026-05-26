# Phase 5 - Samples

## Goal

Create working samples that demonstrate GovernAI usage without connecting to any real AI provider.

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
/copilot/phase-5-samples.md
```

---

## Scope

Create samples only.

Do not implement new SDK features unless required to make samples compile.

---

## Required Samples

```text
samples/MinimalApi.Basic
samples/MinimalApi.MultiTenant
samples/MinimalApi.PolicyDemo
```

---

## Sample 1: `MinimalApi.Basic`

Must demonstrate:

- registering GovernAI
- using GovernAI middleware
- tracking a fake AI call
- console exporter
- returning fake response

No real AI provider.

---

## Sample 2: `MinimalApi.MultiTenant`

Must demonstrate:

- tenant ID from `X-Tenant-Id`
- user ID from a fake/mock claim or explicit context
- event includes tenant ID
- tracking fake AI operation

No real AI provider.

---

## Sample 3: `MinimalApi.PolicyDemo`

Must demonstrate:

- safe prompt allowed
- risky prompt reviewed
- critical prompt denied
- sensitive data redaction example

No real AI provider.

---

## Fake AI Provider

Create a simple fake AI method inside samples:

```csharp
static Task<string> FakeAiCallAsync(string prompt, CancellationToken cancellationToken)
{
    return Task.FromResult($"Fake AI response for: {prompt}");
}
```

Do not call:

- OpenAI
- Azure OpenAI
- Anthropic
- Gemini
- AWS Bedrock

---

## README Updates

Update sample documentation with:

```text
how to run each sample
expected behavior
sample requests
sample responses
```

---

## Acceptance Criteria

- All samples compile.
- All samples run locally.
- No real AI provider used.
- No external dependency added.