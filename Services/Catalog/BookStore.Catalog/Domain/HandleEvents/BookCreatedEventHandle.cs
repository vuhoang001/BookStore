using BookStore.Catalog.Domain.Events;
using BuildingBlocks.Chassis.EventBus.Dispatcher;
using Mediator;

namespace BookStore.Catalog.Domain.HandleEvents;

/// <summary>
/// Domain event handler for BookCreatedEvent.
/// In current architecture, this handler maps/publishes integration event via EventDispatcher.
/// </summary>
public sealed class BookCreatedEventHandle(
    IEventDispatcher eventDispatcher,
    ILogger<BookCreatedEventHandle> logger)
    : INotificationHandler<BookCreatedEvent>
{
    public async ValueTask Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
    {
        await eventDispatcher.DispatchAsync(notification, cancellationToken);
    }
}
