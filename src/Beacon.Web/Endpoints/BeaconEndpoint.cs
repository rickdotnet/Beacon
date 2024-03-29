using Apollo;
using Apollo.Messaging;
using Apollo.Messaging.Abstractions;
using Beacon.Contracts;
using Beacon.Services;

namespace Beacon.Web.Endpoints;

public class BeaconEndpoint : IHandle<CreateComponentCommand>, IListenFor<ComponentCreatedEvent>
{
    private readonly BeaconStore beaconStore;
    private readonly IStateObserver stateObserver;
    private readonly IPublisher publisher;

    public BeaconEndpoint(BeaconStore beaconStore, IPublisherFactory publisherFactory, IStateObserver stateObserver)
    {
        this.beaconStore = beaconStore;
        this.stateObserver = stateObserver;
        publisher = publisherFactory.CreatePublisher("BeaconEndpoint");
    }


    public async Task HandleAsync(CreateComponentCommand message, CancellationToken cancellationToken)
    {
        var component = await beaconStore.AddComponentAsync(message);
        await publisher.BroadcastAsync(
            new ComponentCreatedEvent
            {
                Id = component.Id,
                Name = component.Name,
                Type = component.Type,
                Description = component.Description,
                Environments = component.Environments,
                Tags = component.Tags,
                Owners = component.Owners,
                Links = component.Links
            }, cancellationToken);
    }

    public async Task HandleAsync(ComponentCreatedEvent message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Component created: {message.Name}");
        await stateObserver.NotifyAsync(message);
        // return Task.CompletedTask;
    }
}