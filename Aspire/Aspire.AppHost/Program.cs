using BuildingBlocks.Constants.Core;
using DotNetEnv;

Env.Load();
var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password", secret: true);

var sql = builder.AddSqlServer(Components.SqlServer, password)
    .WithEndpoint(Network.Tcp, e =>
    {
        e.Port       = 14333;
        e.TargetPort = 1433;
    })
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = sql.AddDatabase("CatalogDb");

builder.AddProject<Projects.BookStore_Catalog>(Services.Catalog)
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

builder.Build().Run();
