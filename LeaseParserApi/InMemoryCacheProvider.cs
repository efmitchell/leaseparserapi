using LeaseParserApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LeaseParserApi;

public class InMemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;

    public InMemoryCacheProvider(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGet<T>(string cacheKey, out T value)
    {
        return _cache.TryGetValue(cacheKey, out value);
    }

    public void Set<T>(string cacheKey, T value)
    {
        _cache.Set(cacheKey, value);
    }
}