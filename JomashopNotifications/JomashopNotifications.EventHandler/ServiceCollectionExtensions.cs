using MassTransit;
using JomashopNotifications.EventHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JomashopNotifications.Application.Common;
using JomashopNotifications.EventHandler.WindowsToastNotifications;

namespace JomashopNotifications.EventHandler;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = configuration.GetOptionsOrFail<RabbitMqOptions>(RabbitMqOptions.SectionName);

        return services.AddMassTransit(busRegConfig =>
        {
            busRegConfig.SetKebabCaseEndpointNameFormatter();
            busRegConfig.AddConsumer<ProductInStockEventToastNotificationHandler>();

            busRegConfig.UsingRabbitMq((context, busConfig) =>
            {
                busConfig.Host(
                    rabbitMqOptions.Host,
                    rabbitMqOptions.Port,
                    "/",
                    hc =>
                    {
                        hc.Username(rabbitMqOptions.UserName);
                        hc.Password(rabbitMqOptions.Password);

                        busConfig.ConfigureEndpoints(context);
                    });
            });
        });
    }
}