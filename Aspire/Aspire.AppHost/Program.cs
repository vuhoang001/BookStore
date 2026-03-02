using Aspire.AppHost.Extensions.Infrastructure;
using BuildingBlocks.Constants.Core;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password", secret: true, value: "1234@Abcd");
var registry = builder.AddContainerRegistry();


var queue = builder
    .AddRabbitMQ(Components.Queue)
    .WithIconName("Pipeline")
    .WithManagementPlugin()
    .WithDataVolume()
    .WithImagePullPolicy(ImagePullPolicy.Always)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(Network.Tcp, e => e.Port = 5672);

var sql = builder.AddSqlServer(Components.SqlServer, password)
    .WithEndpoint(Network.Tcp, e =>
    {
        e.Port = 14333;
        e.TargetPort = 1433;
    })
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = sql.AddDatabase(Components.Database.Catalog);

var catalogApi = builder.AddProject<Projects.BookStore_Catalog>(Services.Catalog)
    .WithExternalHttpEndpoints()
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithContainerRegistry(registry);


var basketApi = builder.AddProject<Projects.BookStore_Basket>(Services.Basket)
    .WithExternalHttpEndpoints();


var scalar = builder.AddScalarApiReference()
    .WithContainerRuntimeArgs("--add-host=host.docker.internal:host-gateway");


scalar.WithApiReference(basketApi)
    .WithApiReference(catalogApi);


// Console.WriteLine(basketApi.Resource.Name);


// var gateway = builder
//     .AddApiGatewayProxy()
//     .WithService(basketApi)
//     .WithService(catalogApi);
//
//
// if (builder.ExecutionContext.IsRunMode)
// {
//     builder.AddScalar()
//         .WithOpenAPI(catalogApi)
//         .WithOpenAPI(basketApi);
// }


builder.Build().Run();
