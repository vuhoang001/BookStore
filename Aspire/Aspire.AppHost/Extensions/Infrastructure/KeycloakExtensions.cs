namespace Aspire.AppHost.Extensions.Infrastructure;

public sealed class KeycloakResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpEndpoint;

    public EndpointReference HttpEndpoint =>
        _httpEndpoint ??= new EndpointReference(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"{HttpEndpoint.Property(EndpointProperty.Scheme)}://{HttpEndpoint.Property(EndpointProperty.Host)}:{HttpEndpoint.Property(EndpointProperty.Port)}"
        );
}

public static class KeycloakExtensions
{
    private const string KeycloakImage   = "quay.io/keycloak/keycloak";
    private const string KeycloakVersion = "latest";

    public static IResourceBuilder<KeycloakResource> AddKeycloak(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<PostgresDatabaseResource>? database = null,
        IResourceBuilder<ParameterResource>? adminUser = null,
        IResourceBuilder<ParameterResource>? adminPass = null,
        int port = 8080)
    {
        adminUser ??= builder.AddParameter($"{name}-admin-user", value: "admin", secret: false);
        adminPass ??= builder.AddParameter($"{name}-admin-pass", value: "admin", secret: true);

        var resource = new KeycloakResource(name);

        var rb = builder
            .AddResource(resource)
            .WithImage(KeycloakImage, KeycloakVersion)
            .WithImagePullPolicy(ImagePullPolicy.Always)
            .WithHttpEndpoint(port: port, targetPort: 8080, name: KeycloakResource.HttpEndpointName)
            .WithEnvironment("KEYCLOAK_ADMIN", adminUser)
            .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", adminPass)
            .WithArgs("start-dev")
            .WithIconName("LockClosed")
            .WithHttpHealthCheck()
            ;

        if (database is not null)
        {
            var postgresServer   = (PostgresServerResource)database.Resource.Parent;
            var postgresEndpoint = new EndpointReference(postgresServer, "tcp");

            rb.WithEnvironment("KC_DB", "postgres")
                .WithEnvironment("KC_DB_USERNAME", ReferenceExpression.Create(
                                     $"{postgresServer.UserNameParameter}"
                                 ))
                .WithEnvironment("KC_DB_PASSWORD", ReferenceExpression.Create(
                                     $"{postgresServer.PasswordParameter}"
                                 ))
                .WithEnvironment("KC_DB_URL", ReferenceExpression.Create(
                                     $"jdbc:postgresql://{postgresEndpoint.Property(EndpointProperty.Host)}:{postgresEndpoint.Property(EndpointProperty.TargetPort)}/{database.Resource.DatabaseName}"
                                 ))
                .WithReference(database)
                .WaitFor(database);
        }

        return rb;
    }

    public static IResourceBuilder<KeycloakResource> WithDataVolume(
        this IResourceBuilder<KeycloakResource> builder,
        string? volumeName = null)
        => builder.WithVolume(
            volumeName ?? $"{builder.Resource.Name}-data",
            "/opt/keycloak/data");

    public static IResourceBuilder<KeycloakResource> WithRealmImport(
        this IResourceBuilder<KeycloakResource> builder,
        string realmFilePath)
        => builder
            .WithBindMount(realmFilePath, "/opt/keycloak/data/import/realm.json", isReadOnly: true)
            .WithArgs("--import-realm");

    public static IResourceBuilder<KeycloakResource> WithPersistentLifetime(
        this IResourceBuilder<KeycloakResource> builder)
        => builder.WithLifetime(ContainerLifetime.Persistent);
}
