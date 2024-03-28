using Beacon.Web.Components;

namespace Beacon.Web;

public static class Setup
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var config = BeaconConfig.Default;
        builder.Configuration.Bind(config);


        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        
        return app;
    }
}