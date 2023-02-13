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
        private readonly DistributedCacheEntryOptions _cacheEntryOptions;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetSlidingExpiration(TimeSpan.FromSeconds(60));
        }

        public void Remove(string cacheKey) => _distributedCache.Remove(cacheKey);

        public T Set<T>(string cacheKey, T value)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            _distributedCache.Set(cacheKey, Encoding.UTF8.GetBytes(serializedValue), _cacheEntryOptions);
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

            var deserializedValue = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cacheValue));
            value = deserializedValue;
            return true;
        }

    }
}
