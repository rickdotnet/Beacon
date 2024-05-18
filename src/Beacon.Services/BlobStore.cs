using FusionRocks;
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
        var options = FusionRocksOptions.Default with { CachePath = path, DatabaseMode = true};
        
        rocksDb = new FusionRocks.FusionRocks(options, serializer);
        cache = new FusionCache(new FusionCacheOptions
        {
            CacheName = "blob-store",
            DefaultEntryOptions = new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromDays(1),
                IsFailSafeEnabled = true, // in conjunction with database mode, this *should* turn our L2 cache into a non-expiring database
                DistributedCacheDuration = TimeSpan.MaxValue // shouldn't be necessary, but just in case
            }
        });

        cache.SetupDistributedCache(rocksDb, serializer);
    }
    public ValueTask<T> GetOrSetAsync<T>(string key, T value, FusionCacheEntryOptions? options = null)
        => cache.GetOrSetAsync(key, value, options);

    public ValueTask SetAsync<T>(string key, T value, FusionCacheEntryOptions? options = null) 
        => cache.SetAsync(key, value, options);

    public ValueTask SetLevel2Async<T>(string key, T value, FusionCacheEntryOptions? options = null)
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