using Beacon.Contracts;
using Beacon.Contracts.Models;
using Beacon.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace Beacon.Services;

/// <summary>
/// Transactional data store for the Beacon application.
/// </summary>
public class BeaconStore : IDisposable, IAsyncDisposable
{
    public IQueryable<Component> Components 
        => context.Components;

    private BeaconContext context;

    public BeaconStore(IDbContextFactory<BeaconContext> contextFactory) 
        => context = contextFactory.CreateDbContext();

    public async Task<Component> AddComponentAsync(CreateComponentCommand command)
    {
        var component = new Component
        {
            Name = command.Name,
            Type = command.Type,
            Description = command.Description,
            Environments = command.Environments,
            Tags = command.Tags,
            Owners = command.Owners,
            Links = command.Links
        };
        context.Components.Add(component);
        await context.SaveChangesAsync();
        
        return component;
    }

    public void Dispose()
        => context.Dispose();

    public ValueTask DisposeAsync()
        => context.DisposeAsync();
}