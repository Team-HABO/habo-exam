# REST Movie Service

REST API for managing movies, direcotrs and production companies

## Endpoints
 - Movies: GetAll, GetByID, Create, Update and Delete
 - Movies: GetAll, GetByID
 - Movies: GetAll, GetByID
GetAll uses pagination


## Tech Stack
ASP.NET
SQLite db file
Entity Framework Core as ORM

## Run with docker
```bash
cd services/rest
docker compose up -d
```

### SQL Injection
EF Core protects from SQL injection because it uses parameterized queries under the hood, so user input is never treated as raw SQL.

### CRSF
An attacker tricks a user’s browser into sending a request to another website
For modern APIs that store API keys in headers or use OAuth CSRF is not a problem

### XSS (Cross-Site Scripting)
The XSS process
1. An attacker injects HTML or JavaScript code in a website
2. The website will make a subsequent API call passing the code as a parameter
    • It could be a link, so that the code could even reside in another website
3. The code will be executed by the API server at some point
We sanitize input using /rest/Middleware/XssMiddleware.cs
Middleware is applied in program.cs with app.UseMiddleware<XssMiddleware>();
Content-Security-Policy (CSP) header is added to every response, tells the browser which resources (scripts, styles, images) are allowed to be loaded and executed on a specific page
How CSP header protects against XSS

XSS works by tricking a browser into executing malicious code. CSP prevents this in three main ways:
1. Blocking Inline Scripts

By default, a strong CSP blocks "inline" scripts—code written directly inside <script> tags or HTML attributes (like onclick).

    The Attack: An attacker stores <script>alert('xss')</script> in your database.

    The Protection: Even if the browser renders that tag, the CSP says "I don't allow inline scripts," and the browser ignores it.

2. Restricting Trusted Domains

If you do need scripts, you can specify exactly where they come from.

    The Attack: An attacker tries to load a malicious file: <script src="https://evil-hacker.com/steal.js"></script>.

    The Protection: Your CSP might say script-src 'self'. The browser checks the source, sees it isn't from your domain, and blocks the request.

3. Disabling eval()

CSP prevents the use of eval(), which turns strings into executable code. This closes a major loophole used by hackers to sneak scripts past basic filters.

default-src 'none': Allow nothing but HTML unless explicitly allowed (no scripts, CSS, images, API calls, fonts, WebSocket connections, etc.)

frame-ancestors 'none': Prevent the API from being embedded (e.g., <iframe>, <embed>)

base-uri 'none': Prevent relative links to resolve to an attacker domain (e.g., by using a <base href="https://evil.com"> tag


### Version strategy
This API is using URI versioning, meaning the version is in the path api/v1/movies

### CORS
CORS is cinfigured in program.cs where it allows any method from client "http://localhost:3000"
CORS is applied in program.cs with app.UseCors("AllowFrontend");


### to do
.gitignore?

OAuth2
JWT
what endpoint should have Authorize
In Arturos powerpoint he mentions filtering not search
Choose a revocation strategy - i think blacklist because then the api is still stateless
HATEOAS on getall?
OpenAPI documentation 
postman tests
CI pipeline
Finnish readme