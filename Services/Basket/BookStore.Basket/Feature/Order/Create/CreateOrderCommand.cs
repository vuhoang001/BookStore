using BookStore.Basket.Domain.AggregateModels.OrderAggregate;
using BookStore.Basket.Dtos;
using BookStore.Basket.Grpc.Services.Book;
using BuildingBlocks.Chassis.Exceptions;
using Mediator;

namespace BookStore.Basket.Feature.Order.Create;

public sealed class CreateOrderCommand : ICommand<Guid>
{
    public string BookId { get; set; } = string.Empty;
    public List<CreateOrderLineDto> OrderLines { get; set; } = [];
};

public sealed class CreateOrderHandler(IOrderRepository orderRepository, IBookService bookService)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var res = await bookService.GetBookByIdAsync(command.BookId, cancellationToken);
        if (res is null) throw new NotFoundException("May dau roi con oi ");
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
