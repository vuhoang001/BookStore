using BookStore.Catalog.Domain.AggregatesModel.CategoryAggregate;
using BookStore.Catalog.Domain.Events;

namespace BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

public sealed class Book() : AuditableEntity, IAggregateRoot, ISoftDelete
{
    public Book(string? name,
        string? description,
        string? image,
        decimal price,
        decimal? priceSale,
        Guid? categoryId,
        Guid? publisherId,
        Guid[] authorIds
    ) : this()
    {
        Name         = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        Description  = description;
        Image        = image;
        Price        = new Price(price, priceSale);
        Status       = Status.InStock;
        CategoryId   = categoryId;
        PublisherId  = publisherId;
        _bookAuthors = [.. authorIds.Select(authorId => new BookAuthor(authorId))];
    }

    private readonly List<BookAuthor> _bookAuthors = [];
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string? Image { get; private set; }

    public Price? Price { get; private set; }

    public Status Status { get; private set; }

    public double AverageRating { get; private set; }

    public int TotalReviews { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public Guid? PublisherId { get; private set; }

    public IReadOnlyCollection<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();


    public bool IsDeleted { get; set; }

    public void Delete()
    {
        IsDeleted = true;
        RegisterDomainEvent(new BookChangedEvent($"{nameof(Book).ToLowerInvariant()}:{Id}"));
    }

    public Book SetMetadata(
        string? description,
        Guid categoryId,
        Guid publisherId,
        Guid[] authorIds
    )
    {
        Description = !string.IsNullOrWhiteSpace(description)
            ? description
            : throw new CatalogDomainException("Book description is required.");
        CategoryId  = categoryId;
        PublisherId = publisherId;
        _bookAuthors.AddRange(authorIds.Select(authorId => new BookAuthor(authorId)));
        RegisterDomainEvent(new BookCreatedEvent(this));
        return this;
    }

    public Book Update(
        string name, string? description,
        decimal price,
        decimal? priceSale,
        string? image,
        Guid categoryId,
        Guid publisherId,
        Guid[] authorIds
    )
    {
        var isChanged =
            string.Compare(Name, name, StringComparison.OrdinalIgnoreCase)                  != 0
            || string.Compare(Description, description, StringComparison.OrdinalIgnoreCase) != 0;

        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new CatalogDomainException("Book name is required.");
        Description = description;
        Price       = new(price, priceSale);
        CategoryId  = categoryId;
        PublisherId = publisherId;
        Image       = image;
        _bookAuthors.Clear();
        _bookAuthors.AddRange(authorIds.Select(authorId => new BookAuthor(authorId)));

        if (isChanged)
        {
            RegisterDomainEvent(new BookUpdatedEvent(this));
        }

        RegisterDomainEvent(new BookChangedEvent($"{nameof(Book).ToLowerInvariant()}:{Id}"));
        return this;
    }
    public Book RemoveRating(int rating)
    {
        if (TotalReviews <= 1)
        {
            AverageRating = 0;
            TotalReviews  = 0;
        }
        else
        {
            AverageRating = ((AverageRating * TotalReviews) - rating) / (TotalReviews - 1);
            TotalReviews--;
        }

        RegisterDomainEvent(new BookChangedEvent($"{nameof(Book).ToLowerInvariant()}:{Id}"));
        return this;
    }

    
}
