using BookStore.Shared.Grpc.Book.V1;

namespace BookStore.Basket.Grpc.Services.Book;

public interface IBookService
{
    Task<GetBookResponse?> GetBookByIdAsync(string id, CancellationToken cancellationToken = default);
}
