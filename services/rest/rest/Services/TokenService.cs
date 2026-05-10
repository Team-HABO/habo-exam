using StackExchange.Redis;

namespace rest.Services
{
    public class TokenService : ITokenService
    {
        private readonly IDatabase _redis;

        public TokenService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task BlacklistTokenAsync(string jwtId, TimeSpan expiry)
        {
            await _redis.StringSetAsync($"blacklist:{jwtId}", "true", expiry);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string jwtId)
        {
            return await _redis.KeyExistsAsync($"blacklist:{jwtId}");
        }
    }
}
