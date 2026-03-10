using BookStore.Basket.Domain.Events;
using Mediator;

namespace BookStore.Basket.Domain.EventHandler;

public class OrderCreateHandle : INotificationHandler<OrderCreateEvent>
{
    public ValueTask Handle(OrderCreateEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"OrderCreateHandle: Received OrderCreateEvent for Order {notification.OrderCode}");
        return ValueTask.CompletedTask;
    }
}
