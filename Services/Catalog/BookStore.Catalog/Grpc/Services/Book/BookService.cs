using BookStore.Shared.Grpc.Book.V1;
using Grpc.Core;
using Status = Grpc.Core.Status;

namespace BookStore.Catalog.Grpc.Services.Book;

public class BookService(IBookRepository bookRepository) : BookGrpcService.BookGrpcServiceBase
{
    public override async Task<GetBookResponse> GetBook(GetBookRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid book id format"));

        var book = await bookRepository.GetBookByIdAsync(id, context.CancellationToken);

        return book is null
            ? throw new RpcException(new Status(StatusCode.NotFound, $"Book '{request.Id}' not found"))
            : MapToResponse(book);
    }

    public override async Task<GetBooksResponse> GetBooks(GetBooksRequest request, ServerCallContext context)
    {
        var ids = request.Ids
            .Where(id => Guid.TryParse((string)id, out _))
            .Select(id => Guid.Parse((string)id));

        var books = await bookRepository.GetBooksByIdsAsync(ids, context.CancellationToken);

        var response = new GetBooksResponse();
        response.Books.AddRange(books.Select(MapToResponse));
        return response;
    }

    private static GetBookResponse MapToResponse(Domain.AggregatesModel.BookAggregate.Book book) =>
        new()
        {
            Id = book.Id.ToString(),
            Name = book.Name ?? string.Empty,
            Description = book.Description ?? string.Empty,
            Price = (double)(book.Price?.OriginalPrice ?? 0)
        };
}
