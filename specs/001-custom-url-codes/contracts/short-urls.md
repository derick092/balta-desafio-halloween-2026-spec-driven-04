# Short URL API Contract

## Create a short URL

`POST /urls`

Request body:

```json
{
  "url": "https://example.com/path",
  "code": "my-link"
}
```

`code` is optional and nullable. If omitted or `null`, the API generates a code as it does today. A supplied code must contain only unaccented ASCII letters, digits, hyphens, or underscores and be 3-32 characters long. Codes matching reserved first-level route names (`urls`, `openapi`, case-insensitively) are not allowed.

### Responses

- `201 Created`: Existing `ShortUrlResponse` shape, with the selected or generated code. The response fields remain `code`, `shortUrl`, `originalUrl`, `createdAt`, and `clicks`.
- `400 Bad Request`: `ValidationProblem` for invalid URL or custom code, with Portuguese error text.
- `409 Conflict`: `ProblemDetails` when the selected code is already in use, including a unique-index race detected during save.

## Existing operations

No route or response changes:

- `GET /urls`: list generated and custom-code URLs.
- `GET /urls/{code}`: return details using the existing response shape.
- `GET /{code}`: redirect to the original URL and increment clicks.

The code is immutable after creation. Editing, deletion, authentication, and expiration are out of scope.
