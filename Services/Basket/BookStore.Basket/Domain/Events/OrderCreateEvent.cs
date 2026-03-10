using BuildingBlocks.SharedKernel.SeedWork;

namespace BookStore.Basket.Domain.Events;

public sealed class OrderCreateEvent(string orderCode) : DomainEvent
{
    public string OrderCode { get; set; } = orderCode;
};
