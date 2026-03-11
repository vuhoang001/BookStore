using Aspire.ServiceDefaults.WebConfigurations;
using BookStore.Basket.Grpc.Services.Book;
using BookStore.Shared.Grpc.Book.V1;
using BuildingBlocks.Chassis.Exceptions;
using BuildingBlocks.Chassis.Utils;
using BuildingBlocks.Constants.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookStore.Basket.Grpc;

public static class Extensions
{
    public static void AddGrpcServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;


        builder.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });
        services.AddGrpcHealthChecks();

        services.AddGrpcServiceReference<BookGrpcService.BookGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(Http.Schemes.Https)
                .WithHost(BuildingBlocks.Constants.Aspire.Services.Catalog)
                .Build(), HealthStatus.Degraded
        );

        services.AddScoped<IBookService, BookService>();
    }
}
