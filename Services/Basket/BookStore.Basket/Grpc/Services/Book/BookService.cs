
using BookStore.Shared.Grpc.Book.V1;

namespace BookStore.Basket.Grpc.Services.Book;

public class BookService(BookGrpcService.BookGrpcServiceClient bookGrpcServiceClient) : IBookService
{
    public async Task<GetBookResponse?> GetBookByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var result =
            await bookGrpcServiceClient.GetBookAsync(
                new GetBookRequest { Id = id },
                cancellationToken: cancellationToken
            );
        return result;
    }
}
