# Phase 7 - CI/CD

## Goal

Prepare GovernAI repository for OSS-ready build, test, and package validation.

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
/copilot/phase-7-cicd.md
```

---

## Scope

Add CI/CD readiness only.

Do not publish NuGet packages in this phase.

---

## Required Files

Create or update:

```text
.github/workflows/build.yml
.editorconfig
.gitignore
LICENSE
CONTRIBUTING.md
Directory.Build.props
```

---

## GitHub Actions Workflow

The workflow must:

- run on pull request
- run on main branch push
- restore solution
- build solution
- run tests
- pack NuGet packages
- upload package artifacts if appropriate
- not publish to NuGet

---

## .NET SDK Versions

Use:

```text
.NET 8 SDK
.NET 10 SDK
```

---

## Required Commands

Workflow should run equivalent of:

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet pack --configuration Release --no-build
```

---

## Packaging Requirements

Packages:

```text
GovernAI.Abstractions
GovernAI.Core
GovernAI.Security
GovernAI.AspNetCore
```

Do not publish packages.

---

## OSS Files

### LICENSE

Use MIT unless changed by repository owner.

### CONTRIBUTING.md

Include:

- how to build
- how to test
- coding guidelines
- dependency policy
- security-first contribution expectations

---

## Acceptance Criteria

- CI file is valid.
- Build step exists.
- Test step exists.
- Pack step exists.
- No publish step.
- Repository is ready for PR validation.