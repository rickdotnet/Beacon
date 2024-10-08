using Apollo.Configuration;
using Apollo.Extensions.Microsoft.Hosting;
using Beacon.Services;
using Beacon.Web.Components;
using Beacon.Web.Endpoints;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Components.Tooltip;

namespace Beacon.Web;

public static class Setup
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddFluentUIComponents();
        builder.Services.AddDataGridEntityFrameworkAdapter();

        builder.Services.AddBeaconServices();
        builder.Services.AddHostedService<HealthCheckRefresher>();
        builder.Services.AddApollo(
            new ApolloConfig { DefaultConsumerName = "default" },
            ab => ab
                .AddEndpoint<BeaconEndpoint>(BeaconEndpoint.EndpointConfig)
                .AddEndpoint<HealthCheckEndpoint>(HealthCheckEndpoint.EndpointConfig)
               // .AddNatsProvider(opts => opts with { Url = "nats://localhost:4222" })
        );
        builder.Services.AddSingleton<IStateObserver, StateObserver>();
        builder.Services.AddScoped<ITooltipService, TooltipService>();

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