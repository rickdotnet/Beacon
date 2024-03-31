using Beacon.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace Beacon.Services.Data;

public class ComponentStore
{
    private readonly BeaconStore beaconStore;
    private readonly BlobStore blobStore;

    public ComponentStore(BeaconStore beaconStore, BlobStore blobStore)
    {
        this.beaconStore = beaconStore;
        this.blobStore = blobStore;
    }

    public async Task<Component?> GetComponentAsync(Guid componentId)
    {
        var component = await beaconStore.Components.FirstOrDefaultAsync(x=>x.Guid == componentId);
        return component;
    }
}