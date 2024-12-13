using System;
using Hangfire.Logging;
using Microsoft.Extensions.Caching.Memory;
using SkyNile.Services.Interfaces;
namespace SkyNile.Services;

public class CacheService : ICacheService
{
    private IMemoryCache _cache;
    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
        
    }

    public T? GetData<T>(string key)
    {
        if (_cache.TryGetValue(key, out T? data))
            return data;
        return default;
    }

    public bool RemoveData(string key)
    {
        var res = true;
        if (!string.IsNullOrEmpty(key))
            _cache.Remove(key);
        else
            res = false;
        return res;
    }

    public bool SetData<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null, CacheItemPriority priority = CacheItemPriority.Normal)
    {
        TimeSpan slidingExpirationValid = slidingExpiration is not null ? (TimeSpan)slidingExpiration : TimeSpan.FromSeconds(300);
        TimeSpan absoluteExpirationValid = absoluteExpiration is not null ? (TimeSpan)absoluteExpiration : TimeSpan.FromMinutes(60);
        var res = true;
        var cacheEntryOptions = new MemoryCacheEntryOptions().
            SetSlidingExpiration(slidingExpirationValid).
            SetAbsoluteExpiration(absoluteExpirationValid).
            SetPriority(priority);
        if (!string.IsNullOrEmpty(key) && value is not null)
            _cache.Set(key, value, cacheEntryOptions);
        else
            res = false;
        return res;
    }
}
