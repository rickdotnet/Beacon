using Apollo;
using Apollo.Abstractions;
using Apollo.Configuration;
using Beacon.Contracts;
using Beacon.Services;

namespace Beacon.Web.Endpoints;

public class BeaconEndpoint : IHandle<CreateComponentCommand>, IListenFor<ComponentCreatedEvent>
{
    private readonly BeaconStore beaconStore;
    private readonly IStateObserver? stateObserver;
    private readonly IPublisher publisher;

    public static readonly EndpointConfig EndpointConfig = new()
    {
        EndpointName = "Beacon Endpoint",
        Subject = "beacon",
    };

    public BeaconEndpoint(BeaconStore beaconStore, ApolloClient apollo, IStateObserver? stateObserver)
    {
        this.beaconStore = beaconStore;
        this.stateObserver = stateObserver;
        publisher = apollo.CreatePublisher(EndpointConfig);
    }

    public async Task Handle(CreateComponentCommand message, CancellationToken cancellationToken)
    {
        var component = await beaconStore.AddComponentAsync(message);
        await publisher.Broadcast(
            new ComponentCreatedEvent
            {
                Id = component.Guid,
                Name = component.Name,
                Type = component.Type,
                Description = component.Description,
                Environments = component.Environments,
                Tags = component.Tags,
                Owners = component.Owners,
                Links = component.Links
            }, cancellationToken);
    }

    public Task Handle(ComponentCreatedEvent message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Component created: {message.Name}");
        return stateObserver?.NotifyAsync(message, cancellationToken) ?? Task.CompletedTask;
    }
}