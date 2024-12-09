using System;
using Microsoft.Extensions.Caching.Memory;
namespace SkyNile.Services.Interfaces;

public interface ICacheService
{
    T? GetData<T>(string key);
    public bool SetData<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null, CacheItemPriority Priority = CacheItemPriority.Normal);
    bool RemoveData(string key);
}
