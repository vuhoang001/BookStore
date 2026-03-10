namespace BookStore.Catalog.Infrastructure.Repositories;

public class BookRepository(CatalogDbContext context) : IBookRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<Guid> AddAsync(Book book, CancellationToken cancellationToken)
    {
        var entry = await context.Books.AddAsync(book, cancellationToken);
        return entry.Entity.Id;
    }

    public void Update(Book book, CancellationToken cancellationToken)
    {
        context.Books.Update(book);
    }


    public async Task<Book?> GetBookAsync()
    {
        var result = await context.Books.FirstOrDefaultAsync();
        return result;
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
