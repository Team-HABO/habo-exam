using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rest.Services;

namespace rest.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;



        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration config, ITokenService tokenService)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var client = _httpClientFactory.CreateClient();

            var auth0Request = new
            {
                grant_type = "password",
                username = request.Username,
                password = request.Password,
                audience = _config["Auth0_Audience"],
                client_id = _config["Auth0_ClientId"],
                client_secret = _config["Auth0_ClientSecret"],
                scope = "openid profile email"
            };
            var response = await client.PostAsJsonAsync($"https://{_config["Auth0_Domain"]}/oauth/token", auth0Request);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized("Invalid credentials or Auth0 configuration error.");
            }

            var auth0Response = await response.Content.ReadFromJsonAsync<dynamic>();
            return Ok(auth0Response);
        }
        [Authorize]
        [HttpPost("logout/{jwtId}")]
        public async Task<IActionResult> Logout(string jwtId)
        {
            // Extract claims from the validated JWT (already authenticated by [Authorize])
            var tokenJti = User.FindFirst("jti")?.Value;

            // Ensure the jwtId in the path matches the token's actual jti claim
            if (tokenJti == null || tokenJti != jwtId)
                return Forbid();

            // Get the token expiry to set Redis TTL (no point storing it longer than needed)
            var expiry = TimeSpan.FromHours(1); // Default if no expClaim
            var expClaim = User.FindFirst("exp")?.Value;
            if (long.TryParse(expClaim, out var expUnix))
            {
                var remaining = DateTimeOffset.FromUnixTimeSeconds(expUnix) - DateTimeOffset.UtcNow;
                if (remaining <= TimeSpan.Zero)
                    return BadRequest(new { message = "Token is already expired." });

                expiry = remaining;
            }
            else
            {
                return Unauthorized(new { error = "Malformed token: missing expiration." });
            }

            await _tokenService.BlacklistTokenAsync(jwtId, expiry);
            return Ok(new { message = "Logged out successfully." });
        }
    }

    public record LoginRequest(string Username, string Password);
}
