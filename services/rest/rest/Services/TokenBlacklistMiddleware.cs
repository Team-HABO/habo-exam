namespace rest.Services
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
        {
            // Only check authenticated requests
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var jti = context.User.FindFirst("jti")?.Value;
                if (jti != null && await tokenService.IsTokenBlacklistedAsync(jti))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }

            await _next(context);
        }
    }
}
