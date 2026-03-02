using Microsoft.AspNetCore.Authentication.OAuth;
using Scalar.Aspire;

namespace Aspire.AppHost.Extensions.Infrastructure;

public static class ScalarExtensions
{
    public static IResourceBuilder<ScalarResource> AddScalar(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResource>? keycloak = null)
    {
        var scalar = builder.AddScalarApiReference(options =>
                                                       options.DisableDefaultFonts().PreferHttpsEndpoint()
                                                           .AllowSelfSignedCertificates());

        return scalar;
    }

    /// <summary>
    ///     Configures the Scalar resource builder to include an API reference with OAuth authorization.
    /// </summary>
    /// <param name="builder">The Scalar resource builder to configure.</param>
    /// <param name="api">The project resource builder representing the API project.</param>
    /// <returns>The configured Scalar resource builder with OAuth authorization.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when Keycloak resource is not found in the application builder or when the required 'kc-realm' parameter is
    ///     not configured.
    /// </exception>
    public static IResourceBuilder<ScalarResource> WithOpenAPI(
        this IResourceBuilder<ScalarResource> builder,
        IResourceBuilder<ProjectResource> api
    )
    {
        return builder.WithApiReference(
            api,
            async (options, ctx) =>
            {
                var clientId = api.Resource.Name;

                var parameter = builder
                    .ApplicationBuilder.Resources.OfType<ParameterResource>()
                    .FirstOrDefault(r =>
                                        string.Equals(
                                            r.Name,
                                            $"{clientId}-secret",
                                            StringComparison.OrdinalIgnoreCase
                                        )
                    );

                if (parameter is not null)
                {
                    var clientSecret = await parameter.GetValueAsync(ctx);

                    options
                        .AddPreferredSecuritySchemes([OAuthDefaults.DisplayName])
                        .AddAuthorizationCodeFlow(
                            OAuthDefaults.DisplayName,
                            flow =>
                            {
                                flow.WithClientId(clientId);

                                if (!string.IsNullOrWhiteSpace(clientSecret))
                                {
                                    flow.WithClientSecret(clientSecret);
                                }
                            }
                        );
                }
            }
        );
    }
}
