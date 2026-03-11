using BookStore.Basket.Infrastructure.ReadModels.Books;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Basket.Infrastructure.Repositories;

public class BookReadModelRepository(BasketDbContext context) : IBookReadModelRepository
{
    public async Task<List<BookReadModel>> GetAllBooksAsync(CancellationToken cancellationToken)
    {
        var result = await context.BookReadModel.ToListAsync(cancellationToken);
        return result;
    }

    public async Task<BookReadModel?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await context.BookReadModel.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return result;
    }

    public async Task AddBookAsync(BookReadModel bookReadModel, CancellationToken cancellationToken)
    {
        context.BookReadModel.Add(bookReadModel);

        await context.SaveChangesAsync(cancellationToken);
    }
}
