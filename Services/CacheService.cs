using StackExchange.Redis;
using System.Text.Json;

namespace CachingWebApi.Services;
public class CacheService : ICacheService
{
    IDatabase _cacheDb;

    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _cacheDb = redis.GetDatabase();
    }
    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);

        if (!string.IsNullOrWhiteSpace(value))
        {
            return JsonSerializer.Deserialize<T>(value);
        }
        else { return default; }
    }

    public object RemoveData(string key)
    {
        var _exist = _cacheDb.KeyExists(key);

        if (_exist)
        {
            return _cacheDb.KeyDelete(key);
        }
        else
        {
            return false;
        }
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);

        return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expirtyTime);
    }

    public void ClearAll()
    {
        _cacheDb.Execute("FLUSHDB");
    }
}