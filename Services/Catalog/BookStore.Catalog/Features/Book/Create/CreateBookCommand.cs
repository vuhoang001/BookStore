using BookStore.Catalog.Domain.Events;
using BuildingBlocks.Chassis.EventBus.Dispatcher;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BookStore.Catalog.Features.Book.Create;

public sealed record CreateBookCommand(
    string Name,
    string Description,
    string? Image,
    decimal Price,
    decimal? PriceSale,
    Guid? CategoryId,
    Guid? PublisherId,
    Guid[] AuthorIds) : ICommand<Guid>;

public sealed class CreateBookCommandHandler(
    IBookRepository bookRepository,
    ILogger<CreateBookCommandHandler> logger,
    IPublishEndpoint bus,
    IEventMapper eventMapper
)
    : ICommandHandler<CreateBookCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var book = new Domain.AggregatesModel.BookAggregate.Book(
            command.Name,
            command.Description,
            command.Image,
            command.Price,
            command.PriceSale,
            command.CategoryId,
            command.PublisherId,
            command.AuthorIds
        );

        var result = await bookRepository.AddAsync(book, cancellationToken);

        logger.LogInformation("CreateBookCommandHandler: Calling SaveChangesAsync");
        await bookRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return result;
    }
}
