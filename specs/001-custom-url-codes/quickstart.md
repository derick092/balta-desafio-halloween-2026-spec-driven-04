# Quickstart: Validar códigos personalizados

## Prerequisites

- .NET 10 SDK.
- Run the API from `src/EncurtaUrl/src/EncurtaUrl.Api` with `dotnet run`; the configured SQLite database is initialized through the existing `EnsureCreated` flow.
- Use `EncurtaUrl.Api.http` in an HTTP client such as the VS Code REST Client extension. Its `@baseUrl` should match the API's listening URL.

## Scenarios

Execute the relevant requests in `EncurtaUrl.Api.http`:

1. **Compatibility**: create a URL without `code`; expect `201 Created` and an automatically generated code.
2. **Custom code**: create with a unique valid `code`; expect `201 Created` and exactly that code in the response URL.
3. **Validation**: submit an invalid-format or out-of-range code, then a reserved route name; expect `400 Bad Request` with a Portuguese validation explanation.
4. **Duplicate**: submit the same custom code twice; expect the first request to succeed and the second to return `409 Conflict` with ProblemDetails. To exercise the save-time race path rather than the availability pre-check, issue concurrent requests with the same code.
5. **Existing behavior**: use the created code with `GET /{code}`; expect a redirect and one additional click. Confirm both custom and generated codes appear in `GET /urls` and `GET /urls/{code}`.
6. **Build**: from the API project directory, run `dotnet build`; expect a successful build with no new warnings.

See [the API contract](./contracts/short-urls.md) and [the data model](./data-model.md) for request, response, and persistence details.
