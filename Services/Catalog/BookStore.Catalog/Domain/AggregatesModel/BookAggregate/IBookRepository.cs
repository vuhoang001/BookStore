namespace BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

public interface IBookRepository : IRepository<Book>, IUnitOfWork
{
    Task<Guid> AddAsync(Book book, CancellationToken cancellationToken);

    void Update(Book book, CancellationToken cancellationToken);

    Task<Book?> GetBookAsync();
}
