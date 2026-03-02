using Aspire.ServiceDefaults.WebConfigurations;
using BookStore.Catalog.Infrastructure;
using BuildingBlocks.Chassis.CQRS.Pipelines;
using BuildingBlocks.Chassis.EventBus;
using BuildingBlocks.Chassis.Exceptions;
using MassTransit;
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

        #region Add exception handlers

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        #endregion


        #region Config Eventbus

        builder.AddEventBus(typeof(ICatalogApiMarker), cfg =>
        {
            cfg.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            cfg.AddConfigureEndpointsCallback((context, _, configurator) =>
                                                  configurator.UseEntityFrameworkOutbox<CatalogDbContext>(context)
            );
        });

        #endregion


        services.AddVersioning();
        services.AddEndpoints(typeof(ICatalogApiMarker));
    }
}
