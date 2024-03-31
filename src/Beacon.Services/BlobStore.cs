using Beacon.Contracts;
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
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var beacon = Path.Combine(folder, "Beacon");
        var path = Path.Combine(beacon, "blob-store"); // TODO: this needs configured
        var serializer = new FusionCacheSystemTextJsonSerializer();
        var options = FusionRocksOptions.Default with { CachePath = path };
        
        rocksDb = new FusionRocks.FusionRocks(options, serializer);
        cache = new FusionCache(new FusionCacheOptions
        {
            CacheName = "blob-store",
            DefaultEntryOptions = new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromDays(7),
                IsFailSafeEnabled = true,
                DistributedCacheDuration = TimeSpan.MaxValue
            }
        });

        cache.SetupDistributedCache(rocksDb, serializer);
    }
    public ValueTask<T> GetOrSetAsync<T>(string key, T value, FusionCacheEntryOptions? options = null)
        => cache.GetOrSetAsync(key, value);

    public ValueTask SetAsync<T>(string key, T value, FusionCacheEntryOptions? options = default) 
        => cache.SetAsync(key, value, options ?? new FusionCacheEntryOptions() );

    public ValueTask SetLevel2Async<T>(string key, T value, FusionCacheEntryOptions? options = default)
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

    public ValueTask<TValue?> GetOrDefaultAsync<TValue>(string key, TValue? defaultValue = default,
        FusionCacheEntryOptions? options = null, CancellationToken token = default)
    {
        return cache.GetOrDefaultAsync(key, defaultValue, options, token);
    }

    public TValue? GetOrDefault<TValue>(string key, TValue? defaultValue = default(TValue?),
        FusionCacheEntryOptions? options = null, CancellationToken token = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}