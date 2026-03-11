namespace BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

public interface IBookRepository : IRepository<Book>, IUnitOfWork
{
    Task<Guid> AddAsync(Book book, CancellationToken cancellationToken);

    void Update(Book book, CancellationToken cancellationToken);

    Task<Book?> GetBookAsync();

    Task<Book?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
