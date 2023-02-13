using Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
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
        private const int EXPIRATION_TIME_IN_SECONDS = 3600;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public void Remove(string cacheKey) => _distributedCache.Remove(cacheKey);

        public T Set<T>(string cacheKey, T value)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            var byteArray = Encoding.UTF8.GetBytes(serializedValue);
            _distributedCache.Set(cacheKey, byteArray, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(EXPIRATION_TIME_IN_SECONDS)
            });
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
            value = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cacheValue));
            return true;
        }

    }
}
