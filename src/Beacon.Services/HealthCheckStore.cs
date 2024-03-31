using Beacon.Contracts;
using Beacon.Contracts.Models;
using Microsoft.Extensions.Caching.Memory;
using ZiggyCreatures.Caching.Fusion;

namespace Beacon.Services;

public class HealthCheckStore
{
    private readonly BeaconStore beaconStore;
    private readonly BlobStore blobStore;

    public HealthCheckStore(BeaconStore beaconStore, BlobStore blobStore)
    {
        this.beaconStore = beaconStore;
        this.blobStore = blobStore;
    }
    
    public async ValueTask AddHealthCheck(HealthCheckSignaledEvent value)
    {
        var healthCheck = await beaconStore.AddHealthcheckAsync(value);
        
        // for quick UI lookup - Guid, lighter entity
        await blobStore.SetAsync($"health:{value.ComponentId}", healthCheck);
        
        // for long-term health check history  - ulong, full health check
        await blobStore.SetAsync($"health:{healthCheck.Id}", value); // SetLevel2Async perhaps?
    }
    
    public  ValueTask<Healthcheck?> GetHealthCheckOrDefault(Guid componentId) 
        => blobStore.GetOrDefaultAsync<Healthcheck>($"health:{componentId}");
    
    public  ValueTask<HealthCheckSignaledEvent?> GetHealthCheckSignalOrDefault(ulong healthCheckId) 
        => blobStore.GetOrDefaultAsync<HealthCheckSignaledEvent>($"health:{healthCheckId}");
    
    public async ValueTask<bool> IsHealthy(Guid componentId)
    {
        var healthCheck = await GetHealthCheckOrDefault(componentId);
        if(healthCheck is null)
            return false;
        
        return healthCheck.IsHealthy 
               && healthCheck.SignalTime.AddSeconds(30) > DateTime.UtcNow;
    }
}