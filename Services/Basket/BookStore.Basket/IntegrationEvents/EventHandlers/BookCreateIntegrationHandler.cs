using BookStore.Basket.Infrastructure.ReadModels.Books;
using BookStore.Constract.IntegrationEvents;
using MassTransit;

namespace BookStore.Basket.IntegrationEvents.EventHandlers;

public class BookCreateIntegrationHandler(IBookReadModelRepository bookReadModelRepository)
    : IConsumer<BookCreateIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BookCreateIntegrationEvent> context)
    {
        var bookReadModel = new BookReadModel
        {
            Id = context.Message.BookId,
            BookName = context.Message.Name,
            BookDescription = context.Message.Description
        };

        await bookReadModelRepository.AddBookAsync(bookReadModel, context.CancellationToken);
    }
}
