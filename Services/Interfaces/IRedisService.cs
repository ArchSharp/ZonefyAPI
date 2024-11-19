using Application.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IRedisService : IAutoDependencyService
    {
        Task SetCacheAsync(string key, string value, TimeSpan? expiry);
        Task<string> GetCacheAsync(string key);
        Task RemoveCacheAsync(string key);
    }
}
