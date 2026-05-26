# Contributing to GovernAI .NET SDK

Thank you for your interest in contributing to GovernAI!

---

## Development Setup

1. Clone the repository
2. Install .NET 8 SDK and .NET 10 SDK
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet build` to build all projects
5. Run `dotnet test` to run all tests

```bash
git clone https://github.com/DNVerma88/governai-dotnet
cd governai-dotnet
dotnet restore
dotnet build
dotnet test
```

---

## Coding Standards

See [docs/coding-guidelines.md](docs/coding-guidelines.md) for detailed guidelines.

Key principles:
- Follow SOLID, KISS, DRY, YAGNI
- Use async APIs with `CancellationToken` support
- Add XML documentation to all public APIs
- Write unit tests for all new code
- No external NuGet dependencies in SDK source projects

---

## Dependency Policy

**Strictly no external NuGet packages in SDK source projects:**

```text
GovernAI.Abstractions  — no dependencies
GovernAI.Core          — depends only on GovernAI.Abstractions
GovernAI.Security      — depends only on GovernAI.Abstractions + GovernAI.Core
GovernAI.AspNetCore    — depends on GovernAI.* + Microsoft.AspNetCore.App (shared framework only)
```

Do not add:
- Semantic Kernel, LangChain, or any AI SDK
- Serilog, NLog, or any third-party logging
- Newtonsoft.Json (use `System.Text.Json` only)
- Polly, MediatR, AutoMapper, or any utility library
- Azure SDK, AWS SDK, GCP SDK

Test projects and samples may use standard test packages (xunit, coverlet).

---

## Security-First Contributions

All contributions must follow [docs/security-guidelines.md](docs/security-guidelines.md).

Requirements:
- Never store raw prompts or responses unless explicitly configured
- Never log Authorization headers, cookies, or sensitive values
- Never read request body in middleware
- Sensitive data detection is heuristic — do not claim full protection
- Follow OWASP LLM Top 10 guidance
- All security-related changes must include threat model reasoning in the PR description
- Regex patterns used for security scanning must use `[GeneratedRegex]` for AOT compatibility

---

## Pull Request Guidelines

- Open an issue before starting large changes
- Keep PRs focused and small
- All tests must pass on both `net8.0` and `net10.0`
- No PR should reduce test coverage
- Update documentation if public API changes

