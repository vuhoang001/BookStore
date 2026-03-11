using BuildingBlocks.Chassis.Repository;

namespace BookStore.Basket.Infrastructure.ReadModels.Books;

public interface IBookReadModelRepository
{
    Task<List<BookReadModel>> GetAllBooksAsync(CancellationToken cancellationToken);
    Task<BookReadModel?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddBookAsync(BookReadModel bookReadModel, CancellationToken cancellationToken);
}
