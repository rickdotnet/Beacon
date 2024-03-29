using Beacon.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.Services;

public static class Setup
{
    public static void AddBeaconServices(this IServiceCollection services)
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var beacon = Path.Combine(folder, "Beacon");
        var path = Path.Combine(beacon, "beacon.db");
        
        services.AddDbContextFactory<BeaconContext>(options =>
        {
            options.UseSqlite($"Data Source={path}");
        });
        services.AddScoped<BeaconStore>();
    }
}