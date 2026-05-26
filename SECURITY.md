# Security Policy

## Supported Versions

| Version      | Supported |
|--------------|-----------|
| 0.1.0-alpha  | ✅ Yes    |

As a pre-1.0 SDK, only the latest published version receives security fixes.
Once v1.0.0 is released, a formal N-1 minor version support window will be defined.

---

## Reporting a Vulnerability

**Please do not open a public GitHub issue for security vulnerabilities.**

Report security issues privately via GitHub's
[Security Advisories](https://github.com/DNVerma88/governai-dotnet/security/advisories/new)
feature (recommended), or by emailing the maintainer directly.

Include as much detail as possible:

- A description of the vulnerability and its potential impact
- Reproduction steps or a proof-of-concept (no weaponised exploits)
- Affected version(s) and component(s)
- Any suggested remediation

We aim to acknowledge all reports within **48 hours** and to provide a resolution
timeline within **7 days**. Critical issues will be patched on a best-effort basis
as quickly as possible.

---

## Scope

This policy covers the four SDK packages:

- `GovernAI.Abstractions`
- `GovernAI.Core`
- `GovernAI.Security`
- `GovernAI.AspNetCore`

The sample projects under `samples/` are for demonstration only and are **not**
production-hardened — vulnerabilities in samples are treated as documentation
bugs, not security vulnerabilities.

---

## Security Design Principles

GovernAI is designed with security and privacy as first-class concerns:

| Principle | Implementation |
|-----------|---------------|
| No raw prompt storage by default | `AllowRawPromptCapture = false` |
| No raw response storage by default | `AllowRawResponseCapture = false` |
| Content hashed, not stored | SHA-256 / HMAC-SHA256 hashing |
| Production guard | `AddGovernAI` throws if raw capture is enabled in Production/Staging |
| Input size limits | All scanners and redactors cap input at 64 KB |
| Log injection prevention | Control characters stripped from all label/error fields |
| Path traversal prevention | `FileExporter` requires absolute paths |
| Tenant spoofing prevention | Claims take priority over headers for authenticated users |

GovernAI provides **heuristic-based governance assistance only**. It does not
guarantee prevention of prompt injection, jailbreaks, or data exfiltration.
Consult your security team for compliance requirements.
