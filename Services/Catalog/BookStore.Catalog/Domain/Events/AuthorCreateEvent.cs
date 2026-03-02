namespace BookStore.Catalog.Domain.Events;

public class AuthorCreateEvent(string name) : DomainEvent
{
    public string Name { get; set; } = name;
}
