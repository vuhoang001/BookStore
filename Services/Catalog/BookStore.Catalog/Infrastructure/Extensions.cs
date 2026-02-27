using BuildingBlocks.Chassis.EF;
using BuildingBlocks.Constants.Aspire;

namespace BookStore.Catalog.Infrastructure;

internal static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddSqlServerDbContext<CatalogDbContext>(connectionName: Components.Database.Catalog, app =>
        {
            services.AddRepositories(typeof(ICatalogApiMarker));
        });

        services.AddMigration<CatalogDbContext>();
    }
}
