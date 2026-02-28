using Aspire.ServiceDefaults.WebConfigurations;
using BuildingBlocks.Chassis.CQRS.Pipelines;
using Mediator;

namespace BookStore.Catalog.Extensions;

internal static class Extensions
{
    public static void AddApplicationService(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();


        services.AddMediator((MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));


        services.AddVersioning();
        services.AddEndpoints(typeof(ICatalogApiMarker));
    }
}
