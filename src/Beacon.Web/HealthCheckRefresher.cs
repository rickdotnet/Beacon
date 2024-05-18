using Apollo.Messaging.Abstractions;
using Apollo.Messaging.Publishing;
using Beacon.Contracts;
using Beacon.Web.Endpoints;

namespace Beacon.Web;

public class HealthCheckRefresher : BackgroundService
{
    private readonly IPublisher localPublisher;
    private readonly TimeSpan DefaultRefreshRate = TimeSpan.FromSeconds(30); 
    
    public HealthCheckRefresher(IPublisherFactory publisherFactory)
    {
        localPublisher = publisherFactory.CreatePublisher(nameof(HealthCheckEndpoint), PublisherType.Local);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(DefaultRefreshRate, stoppingToken);
            try
            {
                await localPublisher.BroadcastAsync(new HealthCheckSignaledEvent
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