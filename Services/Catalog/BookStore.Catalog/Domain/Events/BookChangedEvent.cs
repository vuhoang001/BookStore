
namespace BookStore.Catalog.Domain.Events;

public class BookChangedEvent(string key) : DomainEvent
{
    public string Key { get; init; } = null!;
}
