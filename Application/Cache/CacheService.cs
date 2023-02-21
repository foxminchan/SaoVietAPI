using Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;

namespace Application.Cache
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class CacheService : ICache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _cacheEntryOptions;
        private readonly ISubscriber _redisSubscriber;

        public CacheService(IDistributedCache distributedCache, ISubscriber redisSubscriber)
        {
            _distributedCache = distributedCache;
            _cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(120))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600));
            _redisSubscriber = redisSubscriber;
        }

        public void Remove(string cacheKey)
        {
            _distributedCache.Remove(cacheKey);
            _redisSubscriber.Publish("CacheUpdate", cacheKey);
        }

        public T Set<T>(string cacheKey, T value)
        {
            _distributedCache.Set(cacheKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), _cacheEntryOptions);
            _redisSubscriber.Publish("CacheUpdate", cacheKey);
            return value;
        }

        public T Set<T>(string cacheKey, T value, DistributedCacheEntryOptions options)
        {
            _distributedCache.Set(cacheKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), options);
            _redisSubscriber.Publish("CacheUpdate", cacheKey);
            return value;
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            var cacheValue = _distributedCache.Get(cacheKey);
            if (cacheValue == null)
            {
                value = default!;
                return false;
            }
            value = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cacheValue)) ?? default!;
            return true;
        }

        public void Subscribe(Action<string> handler) => _redisSubscriber.Subscribe("CacheUpdate", (_, cacheKey) => handler(cacheKey));

        public void Unsubscribe() => _redisSubscriber.Unsubscribe("CacheUpdate");
    }
}
