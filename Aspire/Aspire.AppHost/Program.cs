using Aspire.AppHost.Extensions.Infrastructure;
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
        e.Port       = 5672;
        e.TargetPort = 5672;
    });

var pgUser = builder.AddParameter("postgres-user", value: "postgres", secret: false);
var pgPass = builder.AddParameter("postgres-pass", value: "postgres", secret: true);

var postgres = builder.AddPostgres(Components.Postgres, pgUser, pgPass)
    .WithIconName("HomeDatabase")
    .WithEndpoint(Network.Tcp, e =>
    {
        e.Port       = 5433;
        e.TargetPort = 5432;
    });


var sql = builder.AddSqlServer(Components.SqlServer, password)
    .WithEndpoint(Network.Tcp, e =>
    {
        e.Port       = 14333;
        e.TargetPort = 1433;
    })
    .WithLifetime(ContainerLifetime.Persistent);


var catalogDb  = sql.AddDatabase(Components.Database.Catalog);
var basketDb   = sql.AddDatabase(Components.Database.Basket);
var keycloakDb = postgres.AddDatabase(Components.Database.Identity);

var keycloak = builder.AddKeycloak(Components.KeyCloak, keycloakDb)
    .WithPersistentLifetime()
    .WithRealmImport("Containers/Keycloak/realms/realm.json");
// .WithDataVolume();

var catalogApi = builder.AddProject<Projects.BookStore_Catalog>(Services.Catalog)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

var basketApi = builder.AddProject<Projects.BookStore_Basket>(Services.Basket)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(basketDb)
    .WaitFor(basketDb)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(catalogApi)
    .WaitFor(catalogApi);

builder.Build().Run();
