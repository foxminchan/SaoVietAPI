using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;

namespace Application.Middleware
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICache _cache;
        private readonly List<IPAddress> _ipWhiteList;

        public RateLimitingMiddleware(RequestDelegate next, ICache cache, IEnumerable<string> ipWhiteList)
        {
            _next = next;
            _cache = cache;
            _ipWhiteList = ipWhiteList.Select(IPAddress.Parse).ToList();
        }

        public async Task Invoke(HttpContext context)
        {
            var clientIpAddress = context.Connection.RemoteIpAddress;

            if (clientIpAddress != null && !IsIpAddressAllowed(clientIpAddress))
            {
                await _next(context);
                return;
            }

            var (cacheKey, limit, duration) = GetRateLimitingDetails(context.Request.Method, context.Request.Path);

            if (!_cache.TryGet<int>(cacheKey, out var requestCount))
                requestCount = 0;

            if (requestCount >= limit)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded.");
                return;
            }

            _cache.Set(cacheKey, requestCount + 1);

            if (!_cache.TryGet<bool>($"{cacheKey}_expiration_set", out var expirationSet) || !expirationSet)
            {
                _cache.Set($"{cacheKey}_expiration_set", true);
                _cache.Set(cacheKey, requestCount + 1, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(duration)
                });
            }

            await _next(context);
        }

        private static (string CacheKey, int Limit, int Duration) GetRateLimitingDetails(string method, string path)
        {
            var cacheKey = $"rl_{method}_{path}";
            int limit = 0, duration = 0;

            switch (method.ToUpper())
            {
                case "POST":
                    limit = 20;
                    duration = 10;
                    break;
                case "PUT":
                    limit = 15;
                    duration = 15;
                    break;
                case "GET":
                    limit = 10;
                    duration = 15;
                    break;
                case "DELETE":
                    limit = 10;
                    duration = 10;
                    break;
            }
            return (cacheKey, limit, duration);
        }

        private bool IsIpAddressAllowed(IPAddress ipAddress) => _ipWhiteList.Contains(ipAddress);
    }

}
