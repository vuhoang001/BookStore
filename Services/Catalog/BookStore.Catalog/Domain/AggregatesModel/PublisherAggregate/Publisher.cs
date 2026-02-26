namespace BookStore.Catalog.Domain.AggregatesModel.PublisherAggregate;

public class Publisher() : Entity, IAggregateRoot
{
    public Publisher(string name) : this()
    {
        Name = name;
    }

    [DisallowNull] public string? Name { get; private set; }

    public Publisher UpdateName(string name)
    {
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new CatalogDomainException("Publisher name must be provided.");
        return this;
    }
}
