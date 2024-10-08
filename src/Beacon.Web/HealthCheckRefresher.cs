using Apollo;
using Apollo.Abstractions;
using Beacon.Contracts;
using Beacon.Web.Endpoints;

namespace Beacon.Web;

public class HealthCheckRefresher : BackgroundService
{
    private readonly IPublisher publisher;
    private readonly TimeSpan defaultRefreshRate = TimeSpan.FromSeconds(30);

    public HealthCheckRefresher(ApolloClient apollo)
    {
        publisher = apollo.CreatePublisher(HealthCheckEndpoint.EndpointConfig);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(defaultRefreshRate, stoppingToken);
            try
            {
                await publisher.Broadcast(
                    new HealthCheckSignaledEvent
                    {
                        ComponentId = Guid.Empty,
                        ComponentName = "ui-refresh",
                        IsHealthy = true
                    }, stoppingToken);
            }
            catch (Exception)
            {
                Console.WriteLine("Uh oh!");
            }
        }
    }
}