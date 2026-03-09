using Mediator;

namespace BookStore.Catalog.Features.Book.Create;

public class CreateBookEndpoint : IEndpoint<Ok<Guid>, CreateBookCommand, ISender>
{
    public async Task<Ok<Guid>> HandleAsync(CreateBookCommand command, ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/book",
                async (CreateBookCommand command, ISender sender) =>
                    await HandleAsync(command, sender)
            )
            .ProducesPostWithoutLocation<Guid>()
            .WithTags(nameof(Author))
            .WithName(nameof(CreateBookEndpoint))
            .WithSummary("Create Book")
            .MapToApiVersion(Versions.V1);
    }
}
