using BuildingBlocks.Chassis.EventBus;

namespace BookStore.Catalog.IntegrationEvents.Events;

/// <summary>
/// Integration event published when a book changes (e.g., deletion, status change).
/// This is an internal event primarily for logging and analytics purposes.
/// </summary>
public record BookChangeIntegrationEvent : IntegrationEvent
{
    public required Guid BookId { get; set; }
    public required string ChangeKey { get; set; }
    public string? Reason { get; set; }
}

