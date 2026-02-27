using Asp.Versioning;
using BuildingBlocks.Constants.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Chassis.EndPoints;

public static class Extension
{
    public static void AddVersioning(this IServiceCollection service)
    {
        service
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = Versions.V1;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
        
        
    }
}
