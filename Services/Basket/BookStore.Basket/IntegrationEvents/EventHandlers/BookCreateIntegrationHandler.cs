using BookStore.Constract.IntegrationEvents;
using MassTransit;

namespace BookStore.Basket.IntegrationEvents.EventHandlers;

public class BookCreateIntegrationHandler : IConsumer<BookCreateIntegrationEvent>
{
    public Task Consume(ConsumeContext<BookCreateIntegrationEvent> context)
    {
        Console.WriteLine("Toi ten la hoang");
        return Task.CompletedTask;
    }
}
