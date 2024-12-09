using System;
using Microsoft.Extensions.Caching.Memory;
namespace SkyNile.Services.Interfaces;

public interface ICacheService
{
    T? GetData<T>(string key);
    public bool SetData<T>(string key, T value, TimeSpan? slidingExpiration, TimeSpan? absoluteExpiration, CacheItemPriority Priority);
    bool RemoveData(string key);
}
