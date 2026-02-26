using BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

namespace BookStore.Catalog.Domain.Events;

public sealed class BookCreatedEvent(Book book) : DomainEvent
{
    public Book Book { get; init; } = book;
}
