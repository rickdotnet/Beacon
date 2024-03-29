using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Beacon.Services.Data;

/// <summary>
/// Used to apply migrations with EF tools locally
/// </summary>
public class DesignTimeFactory : IDesignTimeDbContextFactory<BeaconContext>
{
    public BeaconContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<BeaconContext>();
        builder.EnableSensitiveDataLogging();

        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var beacon = Path.Combine(folder, "Beacon");
        var path = Path.Combine(beacon, "beacon.db");
        if(!Directory.Exists(beacon))
            Directory.CreateDirectory(beacon);

        builder.UseSqlite($"Data Source={path}");
        
        return new BeaconContext(builder.Options);
    }
}