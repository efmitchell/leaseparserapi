namespace LeaseParserApi.Interfaces;

public interface ICacheProvider
{
    bool TryGet<T>(string cacheKey, out T value);
    void Set<T>(string cacheKey, T value);
}