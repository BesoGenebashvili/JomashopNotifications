﻿using JomashopNotifications.Worker;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        var workerOptions = configuration.GetOptionsOrFail<WorkerOptions>(WorkerOptions.SectionName);

        return services.AddQuartz(configurator =>
                {
                    configurator.AddJob<JomashopDataSyncJob>(
                        o => o.WithIdentity(JomashopDataSyncJob.key));

                    var triggerName = $"{nameof(JomashopDataSyncJob)}-Trigger";

                    configurator.AddTrigger(o => o.ForJob(JomashopDataSyncJob.key)
                                                  .WithIdentity(triggerName) // Temporary, should be: WithIntervalInMinutes(options.RunEveryMinutes)
                                                  .WithSimpleSchedule(b => b.WithIntervalInSeconds(workerOptions.RunEveryMinutes)
                                                                            .RepeatForever()));
                })
                .AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
    }

    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = configuration.GetOptionsOrFail<RabbitMqOptions>(RabbitMqOptions.SectionName);

        return services.AddMassTransit(busRegConfig =>
        {
            busRegConfig.SetKebabCaseEndpointNameFormatter();

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

    private static TOptions GetOptionsOrFail<TOptions>(this IConfiguration configuration, string sectionName) =>
        configuration.GetSection(sectionName)
                     .Get<TOptions>()
                     ?? throw new InvalidOperationException($"Configuration section '{sectionName}' is missing.");
}