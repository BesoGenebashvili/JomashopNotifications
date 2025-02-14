using JomashopNotifications.Application;
using JomashopNotifications.Domain;
using JomashopNotifications.Persistence;
using JomashopNotifications.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;

Console.WriteLine("Hello, World!");

var host = CreateHostBuilder(args).Build();
await host.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, config) =>
            config.SetBasePath(AppContext.BaseDirectory)
                  .AddJsonFile(
                      "appsettings.json",
                      optional: false,
                      reloadOnChange: true)
                  .Build())
        .UseSerilog(
            (context, config) => config.ReadFrom.Configuration(context.Configuration))
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;

            var optionsSection = configuration.GetSection(WorkerOptions.SectionName);

            // MissingConfigurationException
            var options = optionsSection.Get<WorkerOptions>()
                                        ?? throw new InvalidOperationException($"Configuration section '{WorkerOptions.SectionName}' is missing.");

            services.Configure<WorkerOptions>(optionsSection)
                    .AddPersistenceServices(configuration)
                    .AddApplicationServices()
                    .AddDomainServices()
                    .AddQuartz(c =>
                    {
                        c.AddJob<JomashopDataSyncJob>(
                            o => o.WithIdentity(JomashopDataSyncJob.key));

                        var triggerName = $"{nameof(JomashopDataSyncJob)}-Trigger";

                        c.AddTrigger(o => o.ForJob(JomashopDataSyncJob.key)
                                           .WithIdentity(triggerName)
                                           .WithSimpleSchedule(b => b.WithIntervalInMinutes(2) //For app: .WithIntervalInMinutes(options.RunEveryMinutes)
                                                                     .RepeatForever()));
                    }).AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
        });
