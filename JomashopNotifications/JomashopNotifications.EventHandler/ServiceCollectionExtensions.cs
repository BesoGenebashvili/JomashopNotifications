﻿using MassTransit;
using JomashopNotifications.EventHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JomashopNotifications.EventHandler;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = configuration.GetOptionsOrFail<RabbitMqOptions>(RabbitMqOptions.SectionName);

        return services.AddMassTransit(busRegConfig =>
        {
            busRegConfig.SetKebabCaseEndpointNameFormatter();
            busRegConfig.AddConsumer<WindowsToastNotificationHandler>();

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

    // Move this in App.Common
    private static TOptions GetOptionsOrFail<TOptions>(this IConfiguration configuration, string sectionName) =>
        configuration.GetSection(sectionName)
                     .Get<TOptions>()
                     ?? throw new InvalidOperationException($"Configuration section '{sectionName}' is missing.");
}