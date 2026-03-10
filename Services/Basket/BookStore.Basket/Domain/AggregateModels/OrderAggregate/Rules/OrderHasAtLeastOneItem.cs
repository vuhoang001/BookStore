using BuildingBlocks.SharedKernel.Exceptions;

namespace BookStore.Basket.Domain.AggregateModels.OrderAggregate.Rules;

public class OrderHasAtLeastOneItem(Order order) : IBusinessRule
{
    public bool IsBroken()
    {
        return order.OrderLines.Count == 0;
    }

    public string Message => "Order must have at least one item";
}
