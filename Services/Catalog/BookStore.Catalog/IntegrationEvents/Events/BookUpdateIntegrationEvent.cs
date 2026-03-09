using BuildingBlocks.Chassis.EventBus;

namespace BookStore.Catalog.IntegrationEvents.Events;

/// <summary>
/// Integration event published when a book is updated.
/// Contains the updated book information for downstream services.
/// </summary>
public record BookUpdateIntegrationEvent : IntegrationEvent
{
    public required Guid BookId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceSale { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? PublisherId { get; set; }
    public Guid[] AuthorIds { get; set; } = [];
}

