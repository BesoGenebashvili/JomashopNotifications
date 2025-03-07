using JomashopNotifications.Application.Common;
using JomashopNotifications.Worker;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace JomashopNotifications.Worker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        var workerOptions = configuration.GetOptionsOrFail<WorkerOptions>(WorkerOptions.SectionName);

        return services.AddQuartz(configurator =>
                {
                    if (workerOptions.JomashopDataSyncJobOptions.IsActive)
                    {
                        Console.WriteLine($"Configuring [{nameof(JomashopDataSyncJob)}] -> {workerOptions.JomashopDataSyncJobOptions.RunEveryMinutes} min");
                        ConfigureJomashopDataSyncJob(configurator);
                    }

                    if (workerOptions.InStockProductsPublisherJobOptions.IsActive)
                    {
                        Console.WriteLine($"Configuring [{nameof(InStockProductsPublisherJob)}] -> {workerOptions.InStockProductsPublisherJobOptions.RunEveryMinutes} min");
                        ConfigureInStockProductsPublisherJob(configurator);
                    }
                })
                .AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

        void ConfigureJomashopDataSyncJob(IServiceCollectionQuartzConfigurator configurator)
        {
            configurator.AddJob<JomashopDataSyncJob>(
                o => o.WithIdentity(JomashopDataSyncJob.key));

            var triggerName = $"{nameof(JomashopDataSyncJob)}-Trigger";

            configurator.AddTrigger(o => o.ForJob(JomashopDataSyncJob.key)
                                          .WithIdentity(triggerName)
                                          .WithSimpleSchedule(b => b.WithIntervalInMinutes(workerOptions.JomashopDataSyncJobOptions.RunEveryMinutes)
                                                                    .RepeatForever()));
        }

        void ConfigureInStockProductsPublisherJob(IServiceCollectionQuartzConfigurator configurator)
        {
            configurator.AddJob<InStockProductsPublisherJob>(
                o => o.WithIdentity(InStockProductsPublisherJob.key));

            var triggerName = $"{nameof(InStockProductsPublisherJob)}-Trigger";

            configurator.AddTrigger(o => o.ForJob(InStockProductsPublisherJob.key)
                                          .WithIdentity(triggerName)
                                          .WithSimpleSchedule(b => b.WithIntervalInMinutes(workerOptions.InStockProductsPublisherJobOptions.RunEveryMinutes)
                                                                    .RepeatForever()));
        }
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
}