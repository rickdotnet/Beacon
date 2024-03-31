using Apollo;
using Apollo.Messaging.Abstractions;
using Beacon.Contracts;
using Beacon.Services;

namespace Beacon.Web.Endpoints;

public class HealthCheckEndpoint : IListenFor<HealthCheckSignaledEvent>
{
    private readonly HealthCheckStore healthCheckStore;
    private readonly IStateObserver? stateObserver;

    public HealthCheckEndpoint(HealthCheckStore healthCheckStore, IStateObserver? stateObserver)
    {
        this.healthCheckStore = healthCheckStore;
        this.stateObserver = stateObserver;
    }
    public async Task HandleAsync(HealthCheckSignaledEvent message, CancellationToken cancellationToken = default)
    {
        try
        {
            if(message.ComponentName == "ui-refresh")
            {
                stateObserver?.NotifyAsync(message, cancellationToken);
            }
            else
            {
                await healthCheckStore.AddHealthCheck(message);
                stateObserver?.NotifyAsync(message, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            // ignored
        }

        
    }
}