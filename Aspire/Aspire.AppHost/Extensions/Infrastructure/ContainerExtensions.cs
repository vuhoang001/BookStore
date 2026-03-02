namespace Aspire.AppHost.Extensions.Infrastructure;

public static class ContainerExtensions
{
    /// <summary>
    ///     Adds a container registry resource to the distributed application builder.
    /// </summary>
    /// <param name="builder">The distributed application builder to configure.</param>
    /// <returns>
    ///     An <see cref="IResourceBuilder{ContainerRegistryResource}" /> instance representing the configured container
    ///     registry resource.
    /// </returns>
#pragma warning disable ASPIRECOMPUTE003
    public static IResourceBuilder<ContainerRegistryResource> AddContainerRegistry(
        this IDistributedApplicationBuilder builder
    )
    {
#pragma warning disable ASPIRECOMPUTE003, ASPIREINTERACTION001
        var endpoint = builder
            .AddParameter("container-endpoint")
            .WithCustomInput(_ =>
                                 new()
                                 {
                                     Name = "ContainerEndpointParameter",
                                     Label = "Container Endpoint",
                                     InputType = InputType.Text,
                                     Description = "Enter your container registry endpoint here",
                                 }
            );

        var repository = builder
            .AddParameter("container-repository")
            .WithCustomInput(_ =>
                                 new()
                                 {
                                     Name = "ContainerRepositoryParameter",
                                     Label = "Container Repository",
                                     InputType = InputType.Text,
                                     Description = "Enter your container registry repository here",
                                 }
            );

        var registry = builder.AddContainerRegistry(
            Components.ContainerRegistry,
            endpoint,
            repository
        );

        return registry;
#pragma warning restore ASPIRECOMPUTE003, ASPIREINTERACTION001
    }
}
