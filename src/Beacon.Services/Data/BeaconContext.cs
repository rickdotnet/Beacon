using Beacon.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace Beacon.Services.Data;

public class BeaconContext : DbContext
{
    public DbSet<Component> Components { get; set; }

    public BeaconContext(DbContextOptions<BeaconContext> options) : base(options)
    {
    }
}