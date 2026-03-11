using Aspire.ServiceDefaults.WebConfigurations;
using BookStore.Basket.Grpc;
using BookStore.Basket.Infrastructure;
using BuildingBlocks.Chassis.CQRS.Pipelines;
using BuildingBlocks.Chassis.EndPoints;
using BuildingBlocks.Chassis.EventBus;
using BuildingBlocks.Chassis.Exceptions;
using MassTransit;
using Mediator;

namespace BookStore.Basket.Extensions;

internal static class Extensions
{
    public static void AddApplicationService(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        #region gRPC clients

        builder.AddGrpcServices();

        #endregion


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

        builder.AddEventBus(typeof(IBasketMarket), cfg =>
        {
            cfg.AddEntityFrameworkOutbox<BasketDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(5);
                o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            cfg.AddConfigureEndpointsCallback((context, _, configurator) =>
                                                  configurator.UseEntityFrameworkOutbox<BasketDbContext>(context)
            );
        });

        #endregion


        services.AddVersioning();
        services.AddEndpoints(typeof(IBasketMarket));
    }
}
