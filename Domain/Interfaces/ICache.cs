using Microsoft.Extensions.Caching.Distributed;

namespace Domain.Interfaces
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public interface ICache
    {
        public void Remove(string cacheKey);
        public T Set<T>(string cacheKey, T value);
        public T Set<T>(string cacheKey, T value, DistributedCacheEntryOptions options);
        public bool TryGet<T>(string cacheKey, out T value);
        public void Subscribe(Action<string> handler);
        public void Unsubscribe();
    }
}
