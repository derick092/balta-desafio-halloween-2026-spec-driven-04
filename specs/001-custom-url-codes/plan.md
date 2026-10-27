# Implementation Plan: Códigos Personalizados para URLs

**Branch**: `001-custom-url-codes` | **Date**: 2026-10-26 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification at `specs/001-custom-url-codes/spec.md` and implementation constraints supplied for this plan.

## Summary

Extend `POST /urls` to accept an optional custom code while preserving automatic generation when it is omitted. Validate custom-code syntax, length, and reserved route names in a small `Services/` validator, check availability before insertion, and translate SQLite unique-index races during `SaveChangesAsync` into a typed HTTP 409 ProblemDetails response. Keep listing, details, and redirects unchanged; do not alter the schema or add packages or a test project.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core Minimal APIs, EF Core SQLite, Microsoft.AspNetCore.OpenApi; reuse existing packages only

**Storage**: Existing SQLite database via `AppDbContext`; `ShortUrls.Code` is already required, max length 32, and uniquely indexed

**Testing**: `dotnet build` and runnable success/error examples in `EncurtaUrl.Api.http`; do not add a test project

**Target Platform**: .NET 10 ASP.NET Core web API

**Project Type**: Single-project web service

**Performance Goals**: No quantified performance target is specified; retain the existing request and database access patterns

**Constraints**: Keep existing routes and response fields compatible; custom codes are 3-32 ASCII letters/digits/hyphens/underscores; no schema changes, new NuGet packages, or test project

**Scale/Scope**: One optional request field, a small validation service, create-flow updates, and HTTP examples

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design.*

| Principle | Gate | Plan |
|---|---|---|
| I. Simplicidade Arquitetural | Pass | Keep the single .NET 10 Minimal API and direct `AppDbContext` access. |
| II. Organização Consistente | Pass | Keep request/response records in `Contracts/`, endpoint in `Endpoints/`, and add a small validator plus interface in `Services/`. |
| III. Respostas HTTP Tipadas e Corretas | Pass | Use TypedResults with `ValidationProblem` for invalid input and a typed `Problem` result for HTTP 409. |
| IV. Persistência Simples e Justificada | Pass | Reuse SQLite and the existing unique index; no schema or database-creation changes. |
| V. Compatibilidade de Contratos | Pass | Add `Code` as an optional request field; keep the existing response, list, detail, and redirect contracts. |
| VI. Dependências Justificadas | Pass | Add no NuGet dependency; existing EF Core SQLite provider exposes the required error information. |
| VII. Validação Reproduzível | Pass | Extend `EncurtaUrl.Api.http` with valid, invalid, reserved, duplicate, and automatic-generation cases, then run `dotnet build`. |
| VIII. Idioma Adequado ao Público | Pass | User-facing validation and conflict messages remain in Portuguese; technical identifiers follow existing English naming. |

**Schema impact**: None. The existing `Code` column and unique index are sufficient. Existing databases created with the current schema remain usable.

**Dependency impact**: None.

**Contract impact**: The create request gains an optional nullable `Code`. Existing clients may omit it without behavior change. Responses are unchanged. Invalid custom codes return 400; occupied custom codes return 409.

## Project Structure

### Documentation (this feature)

```text
specs/001-custom-url-codes/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── short-urls.md
```

### Source Code (repository root)

```text
src/EncurtaUrl/src/EncurtaUrl.Api/
├── Contracts/ShortUrlContracts.cs
├── Data/AppDbContext.cs
├── Endpoints/ShortUrlEndpoints.cs
├── Models/ShortUrl.cs
├── Services/
│   ├── ShortCodeGenerator.cs
│   └── [custom-code validator and interface]
├── EncurtaUrl.Api.http
└── Program.cs
```

**Structure Decision**: Extend the existing single API project. The validator belongs alongside the existing short-code generator in `Services/`; no additional project or architectural layer is introduced.

## Complexity Tracking

No constitution violations.
