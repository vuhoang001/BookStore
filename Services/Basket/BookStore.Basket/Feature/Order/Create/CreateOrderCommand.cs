using BookStore.Basket.Domain.AggregateModels.OrderAggregate;
using BookStore.Basket.Dtos;
using Mediator;

namespace BookStore.Basket.Feature.Order.Create;

public sealed class CreateOrderCommand : ICommand<Guid>
{
    public List<CreateOrderLineDto> OrderLines { get; set; } = [];
};

public sealed class CreateOrderHandler(IOrderRepository orderRepository) : ICommandHandler<CreateOrderCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderCode = await orderRepository.GenerateAsync(cancellationToken);
        var order = Domain.AggregateModels.OrderAggregate.Order.NewOrder(
            orderCode, [
                ..command.OrderLines.Select(ol =>
                                            (
                                                ol.BookId,
                                                ol.BookName,
                                                ol.Quantity,
                                                ol.UnitPrice
                                            )
                )
            ]);

        await orderRepository.AddAsync(order, cancellationToken);

        await orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
