using BuildingBlocks.Chassis.EF;
using BuildingBlocks.Constants.Aspire;

namespace BookStore.Catalog.Infrastructure;

internal static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddSqlServerEfDbContext<CatalogDbContext>(Components.Database.Catalog, app =>
        {
            services.AddMigration<CatalogDbContext>();
            services.AddRepositories(typeof(ICatalogApiMarker));
        });

        Console.WriteLine(builder);
    }
}
