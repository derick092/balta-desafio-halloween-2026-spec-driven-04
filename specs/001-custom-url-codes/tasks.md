---
description: "Task list for custom URL codes"
---

# Tasks: Códigos Personalizados para URLs

**Input**: Design documents from `specs/001-custom-url-codes/`

**Prerequisites**: `plan.md`, `spec.md`, `data-model.md`, `contracts/short-urls.md`, `research.md`, `quickstart.md`

**Tests**: No test project or test tasks are planned. Per the project constitution, executable success and error scenarios belong in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http`; validate with `dotnet build`.

**Organization**: Tasks are grouped by user story so each increment can be implemented and checked independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it changes a separate file and has no incomplete task dependency.
- **[Story]**: User story associated with the task.
- Every task names the exact file to change or validate.

## Phase 1: Setup

**Purpose**: Project initialization.

The existing .NET 10 API project, EF Core SQLite dependency, and database setup are already in place; no setup changes are required.

---

## Phase 2: Foundational

**Purpose**: Shared prerequisites that block all user stories.

No additional shared infrastructure, schema changes, or packages are required for this feature.

---

## Phase 3: User Story 1 - Criar e usar uma URL com código personalizado (Priority: P1) - MVP

**Goal**: Accept a valid, available custom code, use it in the short URL, and preserve redirect, click counting, listing, and details behavior.

**Independent Test**: Submit a valid custom code, confirm the creation response contains that exact code, navigate to it and confirm the redirect and one-click increment, then confirm listing and details retain the existing response fields.

### Implementation for User Story 1

- [X] T001 [P] [US1] Add nullable optional `Code` to `CreateShortUrlRequest` in `src/EncurtaUrl/src/EncurtaUrl.Api/Contracts/ShortUrlContracts.cs`, preserving compatibility when the JSON field is omitted or `null`.
- [X] T002 [P] [US1] Add `ICustomShortCodeValidator` and its implementation in `src/EncurtaUrl/src/EncurtaUrl.Api/Services/CustomShortCodeValidator.cs`; accept only ASCII `[A-Za-z0-9_-]` codes of 3-32 characters and reject first-level route names `urls` and `openapi` case-insensitively with Portuguese validation messages.
- [X] T003 [US1] Update `CreateAsync` in `src/EncurtaUrl/src/EncurtaUrl.Api/Endpoints/ShortUrlEndpoints.cs` to validate and use a supplied custom code, retain the existing automatic-generation path when absent, and return the existing `ShortUrlResponse` shape.
- [X] T004 [US1] Add executable HTTP examples in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http` for creating a custom code, redirecting through it, and checking its list and detail responses.

**Checkpoint**: A valid custom code can be created, used for redirect/click tracking, and read through existing operations.

---

## Phase 4: User Story 2 - Receber retorno claro para códigos inválidos ou ocupados (Priority: P2)

**Goal**: Explain invalid, reserved, and duplicate code rejections in Portuguese and prevent duplicate persistence, including a concurrent unique-index race.

**Independent Test**: Try malformed and out-of-range codes, reserved route names with mixed casing, and a duplicate code; confirm invalid/reserved requests return `400 ValidationProblem`, duplicates return `409 ProblemDetails`, and existing data remains unchanged.

### Implementation for User Story 2

- [X] T005 [US2] Update `CreateAsync` in `src/EncurtaUrl/src/EncurtaUrl.Api/Endpoints/ShortUrlEndpoints.cs` to check custom-code availability before insertion and translate only `DbUpdateException` wrapping `SqliteException.SqliteExtendedErrorCode == 2067` during `SaveChangesAsync` into Portuguese `409 ProblemDetails`; allow unrelated persistence errors to propagate.
- [X] T006 [US2] Add executable error examples in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http` for invalid characters, codes below 3 and above 32 characters, reserved route names, and a duplicate code; document concurrent same-code requests as the way to exercise the save-time race path.

**Checkpoint**: Invalid or reserved codes are rejected with clear validation errors, and occupied codes cannot be persisted even under a concurrent insert race.

---

## Phase 5: User Story 3 - Continuar usando códigos gerados e recursos existentes (Priority: P3)

**Goal**: Preserve automatic generation when `code` is omitted and keep generated and custom codes visible through the existing list and details operations.

**Independent Test**: Create a URL without `code`, verify the API returns a generated code, then confirm list and detail requests for both generated and custom codes return the current fields and behavior.

### Implementation for User Story 3

- [X] T007 [US3] Extend the compatibility examples in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http` to cover creation without `code` and list/detail lookups for generated and custom codes, retaining the existing response shape and routes.

**Checkpoint**: Existing clients can continue omitting `code`, and both code types work with list and detail operations.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate the complete feature and its documented scenarios.

- [X] T008 Run the scenarios in `specs/001-custom-url-codes/quickstart.md` using `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http` and run `dotnet build` for `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.csproj`; resolve any regressions or new build warnings in the affected source files.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No changes required; the existing API project is the implementation target.
- **Foundational (Phase 2)**: No new prerequisites; the existing database schema and dependencies are sufficient.
- **User Stories (Phase 3+)**: Depend on the existing project foundation. Complete US1 before US2 because duplicate handling extends the custom-code create flow. US3 follows US1 to validate compatibility for both code types.
- **Polish (Phase 6)**: Depends on the desired user stories being complete.

### User Story Dependencies

- **US1 (P1)**: Depends only on the existing API foundation; delivers the MVP.
- **US2 (P2)**: Depends on US1's custom-code create flow; adds rejection and uniqueness-race behavior.
- **US3 (P3)**: Depends on US1 to exercise generated and custom codes together in the compatibility scenarios.

### Within Each User Story

- For US1, the request contract and validator (T001-T002) can be implemented in parallel; endpoint integration (T003) follows both, then HTTP examples (T004).
- For US2, implement duplicate handling (T005) before adding its HTTP examples (T006).
- For US3, add compatibility scenarios after US1 behavior is available.
- Finish with the quickstart and build validation (T008).

## Parallel Opportunities

- **US1**: T001 (`ShortUrlContracts.cs`) and T002 (`CustomShortCodeValidator.cs`) can run in parallel because they edit different files.
- **US2 and US3**: No within-story parallel work is identified; the endpoint and HTTP scenario changes depend on prior custom-code behavior and share files across story increments.

### Parallel Example: User Story 1

```text
Task: T001 Add optional Code to CreateShortUrlRequest in Contracts/ShortUrlContracts.cs
Task: T002 Add custom-code validation interface and implementation in Services/CustomShortCodeValidator.cs
```

## Implementation Strategy

### MVP First (User Story 1)

1. Complete T001 and T002, then T003 and T004.
2. Validate custom-code creation, redirect/click counting, listing, and details independently.
3. Stop at the US1 checkpoint for the first usable increment.

### Incremental Delivery

1. Deliver US1 as the MVP.
2. Add US2 invalid/reserved/duplicate handling and verify the corresponding HTTP responses.
3. Add US3 automatic-generation and mixed-code compatibility examples.
4. Run all quickstart scenarios and build the API project.
