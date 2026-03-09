using BookStore.Catalog.IntegrationEvents.Events;
using MassTransit;

namespace BookStore.Catalog.IntegrationEvents.EventHandlers;

/// <summary>
/// Consumer handler for BookUpdateIntegrationEvent.
/// This handler processes book update events from the message queue.
/// 
/// Triggered when: A book is updated (metadata, price, categories, authors)
/// 
/// Usage examples:
/// - Update search indexes (Elasticsearch, etc.)
/// - Update caches
/// - Notify other microservices
/// - Update analytics/dashboards
/// </summary>
public class BookUpdateIntegrationHandler(ILogger<BookUpdateIntegrationHandler> logger)
    : IConsumer<BookUpdateIntegrationEvent>
{
    public Task Consume(ConsumeContext<BookUpdateIntegrationEvent> context)
    {
        logger.LogInformation(
            "Consumed BookUpdateIntegrationEvent: MessageId={MessageId}, BookId={BookId}, Name={Name}",
            context.MessageId,
            context.Message.BookId,
            context.Message.Name
        );

        return Task.CompletedTask;
    }
}
