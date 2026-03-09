using BookStore.Catalog.IntegrationEvents.Events;
using MassTransit;

namespace BookStore.Catalog.IntegrationEvents.EventHandlers;

public class BookCreateIntegrationHandler(ILogger<BookCreateIntegrationHandler> logger)
    : IConsumer<BookCreateIntegrationEvent>
{
    public Task Consume(ConsumeContext<BookCreateIntegrationEvent> context)
    {
        logger.LogInformation(
            "Consumed BookCreateIntegrationEvent: MessageId={MessageId}, BookId={BookId}, Name={Name}",
            context.MessageId,
            context.Message.Id,
            context.Message.Name
        );

        return Task.CompletedTask;
    }
}
