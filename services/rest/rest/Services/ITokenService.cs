namespace rest.Services
{
    public interface ITokenService
    {
        Task BlacklistTokenAsync(string jwtId, TimeSpan expiry);
        Task<bool> IsTokenBlacklistedAsync(string jwtId);
    }

}