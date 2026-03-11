using BuildingBlocks.Chassis.EventBus;

namespace BookStore.Constract.IntegrationEvents;

public record BookCreateIntegrationEvent : IntegrationEvent
{
    public Guid BookId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
