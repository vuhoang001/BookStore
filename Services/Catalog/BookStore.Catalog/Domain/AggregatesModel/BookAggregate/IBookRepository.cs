namespace BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

public interface IBookRepository : IRepository<Book>
{
    Task<Guid> AddAsync(Book book, CancellationToken cancellationToken);
}
