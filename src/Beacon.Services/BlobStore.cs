using Beacon.Contracts.Models;
using FusionRocks;
using Microsoft.Extensions.Caching.Memory;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Beacon.Services;

/// <summary>
/// General purpose key-based blob store.
/// </summary>
public sealed class BlobStore : IDisposable
{
    private readonly FusionCache cache;
    private readonly FusionRocks.FusionRocks rocksDb;

    public BlobStore()
    {
        var serializer = new FusionCacheSystemTextJsonSerializer();
        rocksDb = new FusionRocks.FusionRocks(FusionRocksOptions.Default, serializer);
        cache = new FusionCache(new FusionCacheOptions
        {
            CacheName = "blob-store",
            DefaultEntryOptions = new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromDays(7),
                IsFailSafeEnabled = true,
                DistributedCacheDuration = TimeSpan.MaxValue
            },
            
            //CacheKeyPrefix = null,
            //DistributedCacheKeyModifierMode = CacheKeyModifierMode.Prefix,
        });

        cache.SetupDistributedCache(rocksDb, serializer);
    }
    
    public ValueTask<T?> GetOrDefaultAsync<T>(string key) 
        => cache.GetOrDefaultAsync<T>(key);

    public ValueTask<T> GetOrSetAsync<T>(string key, T value, FusionCacheEntryOptions? options = null)
        => cache.GetOrSetAsync(key, value);

    public ValueTask SetAsync<T>(string key, T value, FusionCacheEntryOptions? options = default) 
        => cache.SetAsync(key, value, options ?? new FusionCacheEntryOptions() );

    public ValueTask SetLevel2Async<T>(string key, T value, FusionCacheEntryOptions? options)
    {
        options ??= new FusionCacheEntryOptions();
        options.SkipMemoryCache = true;
        return SetAsync(key, value, options);
    }

    public void Dispose()
    {
        cache.Dispose();
        rocksDb.Dispose();
    }
}
public static class BlobStoreExtensions
{
    public static ValueTask AddHealthCheck(this BlobStore store, HealthCheckSignal value)
        => store.SetAsync($"health:{value.UniqueId}", value);
    
    public static ValueTask<HealthCheckSignal?> GetHealthCheck(this BlobStore store, string uniqueId)
        => store.GetOrDefaultAsync<HealthCheckSignal>($"health:{uniqueId}");
    
    public static async ValueTask<HealthCheckSignal?> GetOrSetHealthCheck(
        this BlobStore store,
        string uniqueId,
        Func<CancellationToken, Task<HealthCheckSignal>> factory,
        FusionCacheEntryOptions? options = null,
        CancellationToken token = default)
        => await store.GetOrSetAsync($"health:{uniqueId}", await factory(token), options);
}