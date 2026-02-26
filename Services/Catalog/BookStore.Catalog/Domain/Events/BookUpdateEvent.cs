using BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

namespace BookStore.Catalog.Domain.Events;

public class BookUpdatedEvent(Book book) : DomainEvent
{
    public Book Book { get; init; } = null!;
}
