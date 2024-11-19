using StackExchange.Redis;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisService(string redisConnectionString)
        {
            Console.WriteLine("redis con string: "+redisConnectionString);
            _redis = ConnectionMultiplexer.Connect(redisConnectionString);
            _database = _redis.GetDatabase();

            //try
            //{
            //    var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
            //    configurationOptions.AbortOnConnectFail = false;  // Avoid immediate failure on connection issues
            //    _redis = ConnectionMultiplexer.Connect(configurationOptions);
            //    _database = _redis.GetDatabase();
            //}
            //catch (Exception ex)
            //{
            //    // Log or handle connection failure
            //    throw new InvalidOperationException("Could not connect to Redis", ex);
            //}
        }

        // Method to get a cache item
        public async Task<string> GetCacheAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        // Method to remove a cache item
        public async Task RemoveCacheAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task SetCacheAsync(string key, string value, TimeSpan? expiry)
        {
            await _database.StringSetAsync(key, value, expiry);
        }
    }
}
