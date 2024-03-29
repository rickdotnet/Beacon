using Beacon.Contracts;
using Beacon.Contracts.Models;
using Beacon.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace Beacon.Services;

public class BeaconStore
{
    public IQueryable<Component> Components 
        => contextFactory.CreateDbContext().Components;
    
    private readonly IDbContextFactory<BeaconContext> contextFactory;
    private BeaconContext Context => contextFactory.CreateDbContext();

    public BeaconStore(IDbContextFactory<BeaconContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task<Component> AddComponentAsync(CreateComponentCommand command)
    {
        var context = await contextFactory.CreateDbContextAsync();
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
}