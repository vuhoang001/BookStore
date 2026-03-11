using BuildingBlocks.Constants.Core;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Chassis.Utils.Configuration;

public static class LaunchProfileExtensions
{
    public static bool IsHttpsLaunchProfile(this IHostApplicationBuilder builder)
    {
        return builder.Configuration["DOTNET_LAUNCH_PROFILE"] == Http.Schemes.Https;
    }

    public static string GetScheme(this IHostApplicationBuilder builder)
    {
        return builder.IsHttpsLaunchProfile() ? Http.Schemes.Https : Http.Schemes.Http;
    }
}
