using BuildingBlocks.Constants.Core;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password", secret: true, value:"1234@Abcd");

var sql = builder.AddSqlServer(Components.SqlServer, password)
    .WithEndpoint(Network.Tcp, e =>
    {
        e.Port = 14333;
        e.TargetPort = 1433;
    })
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = sql.AddDatabase(Components.Database.Catalog);

builder.AddProject<Projects.BookStore_Catalog>(Services.Catalog)
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

builder.Build().Run();
