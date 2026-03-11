using BuildingBlocks.Constants.Core;
using Scalar.Aspire;


var builder = DistributedApplication.CreateBuilder(args);


var password = builder.AddParameter("sql-password", secret: true, value: "1234@Abcd");


var queue = builder.AddRabbitMQ(Components.Queue)
    .WithIconName("Pipeline")
    .WithManagementPlugin()
    .WithDataVolume()
    .WithImagePullPolicy(ImagePullPolicy.Always)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(Network.Tcp, e =>
    {
        e.Port = 5672; // container port
        e.TargetPort = 5672; // machine port cố định
    });

var sql = builder.AddSqlServer(Components.SqlServer, password)
    .WithEndpoint(Network.Tcp, e =>
    {
        e.Port = 14333;
        e.TargetPort = 1433;
    })
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = sql.AddDatabase(Components.Database.Catalog);
var basketDb = sql.AddDatabase(Components.Database.Basket);

var catalogApi = builder.AddProject<Projects.BookStore_Catalog>(Services.Catalog)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

var basketApi = builder.AddProject<Projects.BookStore_Basket>(Services.Basket)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(basketDb)
    .WaitFor(basketDb)
    .WithReference(catalogApi)
    .WaitFor(catalogApi);


var scalar = builder
    .AddScalarApiReference(options =>
    {
        options.DisableDefaultFonts()
            .PreferHttpsEndpoint()
            .AllowSelfSignedCertificates();
    });

// Tự động add OpenAPI endpoints từ các services
scalar.WithApiReference(catalogApi);
scalar.WithApiReference(basketApi);

builder.Build().Run();
