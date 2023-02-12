using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.High)
                .SetSize(1024);
        }

        public void Remove(string cacheKey) => _memoryCache.Remove(cacheKey);

        public T Set<T>(string cacheKey, T value) => _memoryCache.Set(cacheKey, value, _cacheEntryOptions);

        public bool TryGet<T>(string cacheKey, out T value) => _memoryCache.TryGetValue(cacheKey, out value!);
        
    }
}
