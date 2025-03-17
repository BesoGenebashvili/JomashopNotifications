using MassTransit;
using JomashopNotifications.EventHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JomashopNotifications.Application.Common;
using JomashopNotifications.EventHandler.WindowsToastNotifications;
using JomashopNotifications.EventHandler.EmailNotifications;
using Microsoft.Extensions.Options;

namespace JomashopNotifications.EventHandler;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var eventHandlerOptions = configuration.GetOptionsOrFail<EventHandlerOptions>(EventHandlerOptions.SectionName);
        var rabbitMqOptions = configuration.GetOptionsOrFail<RabbitMqOptions>(RabbitMqOptions.SectionName);

        return services.AddMassTransit(busRegConfig =>
        {
            busRegConfig.SetKebabCaseEndpointNameFormatter();

            if (eventHandlerOptions.ProductInStockEventEmailNotificationHandlerOptions is { IsActive: true, EmailOptions: var emailOptions })
            {
                services.AddEmailService(emailOptions);
                busRegConfig.AddConsumer<ProductInStockEventEmailNotificationHandler>();
            }

            if (eventHandlerOptions.ProductInStockEventToastNotificationHandlerOptions.IsActive)
            {
                busRegConfig.AddConsumer<ProductInStockEventToastNotificationHandler>();
            }

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

    private static IServiceCollection AddEmailService(this IServiceCollection services, EmailOptions emailOptions) =>
        services.AddSingleton(_ => new EmailService(
            Options.Create(emailOptions)));
}