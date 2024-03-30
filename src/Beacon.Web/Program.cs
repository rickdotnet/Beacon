using Beacon.Services.Data;
using Beacon.Web;
using Microsoft.EntityFrameworkCore;

var app =
    WebApplication.CreateBuilder(args)
        .ConfigureServices()
        .ConfigurePipeline();


using (var scope = app.Services.CreateScope())
{
    await using var db =
        await scope.ServiceProvider.GetRequiredService<IDbContextFactory<BeaconContext>>().CreateDbContextAsync();

    await db.Database.MigrateAsync();
}

await app.RunAsync();