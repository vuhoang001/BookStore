using BuildingBlocks.Constants.Aspire;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Chassis.EventBus;

public static class Extensions
{
    public static void AddEventBus(this IHostApplicationBuilder builder, Type type,
        Action<IBusRegistrationConfigurator>? busConfigure = null,
        Action<IBusRegistrationConfigurator, IRabbitMqBusFactoryConfigurator>? rabbitMqConfigure = null)
    {
        var connectionString = builder.Configuration.GetConnectionString(Components.Queue);
    }
}
