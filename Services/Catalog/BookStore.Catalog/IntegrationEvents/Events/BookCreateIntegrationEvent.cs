using BuildingBlocks.Chassis.EventBus;

namespace BookStore.Catalog.IntegrationEvents.Events;

public record BookCreateIntegrationEvent : IntegrationEvent
{
    public Guid? OrderId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
