using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Cache
{
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
