using BuildingBlocks.Chassis.EF;
using BuildingBlocks.Chassis.Repository;
using BuildingBlocks.Constants.Aspire;

namespace BookStore.Basket.Infrastructure;

internal static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddSqlServerEfDbContext<BasketDbContext>(Components.Database.Basket, app =>
        {
            services.AddMigration<BasketDbContext>();
            services.AddRepositories(typeof(IBasketMarket));
        });


        // services.AddScoped<IEventDispatcher, EventDispatcher>();
        // services.AddScoped<IEventMapper, EventMapper>();
    }
}
