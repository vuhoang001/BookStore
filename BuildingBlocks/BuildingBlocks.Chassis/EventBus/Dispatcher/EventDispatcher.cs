using BuildingBlocks.SharedKernel.SeedWork;
using MassTransit;

namespace BuildingBlocks.Chassis.EventBus.Dispatcher;

public class EventDispatcher(IBus bus, IEventMapper eventMapper) : IEventDispatcher
{
    public async Task DispatchAsync(DomainEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var integrationEvent = eventMapper.MapToIntegrationEvent(@event) ??
            throw new InvalidOperationException($"No integration event mapping found for '{@event.GetType().Name}'.");

        await bus.Publish(integrationEvent, cancellationToken);
    }
}
