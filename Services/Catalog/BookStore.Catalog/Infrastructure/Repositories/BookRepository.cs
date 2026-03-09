namespace BookStore.Catalog.Infrastructure.Repositories;

public class BookRepository(CatalogDbContext context) : IBookRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<Guid> AddAsync(Book book, CancellationToken cancellationToken)
    {
        var entry = await context.Books.AddAsync(book, cancellationToken);
        return entry.Entity.Id;
    }
}
