namespace rest.Middleware
{
    /// <summary>
    /// Middleware for Input sanitization
    /// </summary>
    public class XssMiddleware
    {
        private readonly RequestDelegate _next;

        public XssMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (body.Contains('<') || body.Contains('>'))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid characters in request body");
                return;
            }

            await _next(context);
        }
    }
}
