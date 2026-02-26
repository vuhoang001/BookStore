using BookStore.Catalog.Domain.AggregatesModel.AuthorAggregate;

namespace BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

public class BookAuthor() : Entity
{
    public BookAuthor(Guid authorId) : this()
    {
        AuthorId = authorId;
    }

    public Guid AuthorId { get; private set; }
    public Author Author { get; private set; } = null!;
    public Book Book { get; private set; } = null!;
}
