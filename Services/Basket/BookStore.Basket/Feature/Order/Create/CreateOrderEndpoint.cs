using BuildingBlocks.Chassis.EndPoints;
using BuildingBlocks.Constants.Core;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookStore.Basket.Feature.Order.Create;

public class CreateOrderEndpoint : IEndpoint<Ok<Guid>, CreateOrderCommand, ISender>
{
    public async Task<Ok<Guid>> HandleAsync(CreateOrderCommand request1, ISender request2,
        CancellationToken cancellationToken = default)
    {
        var result = await request2.Send(request1, cancellationToken);
        return TypedResults.Ok(result);
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (CreateOrderCommand command, ISender sender) =>
                        await HandleAsync(command, sender))
            .ProducesPostWithoutLocation<Guid>()
            .WithTags(nameof(Domain.AggregateModels.OrderAggregate.Order))
            .WithName(nameof(CreateOrderEndpoint))
            .WithSummary("Create Order")
            .WithDescription("Create a new order in the basket system")
            .MapToApiVersion(Versions.V1);
    }
}
