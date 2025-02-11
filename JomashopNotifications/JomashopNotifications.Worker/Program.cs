using JomashopNotifications.Application;
using JomashopNotifications.Persistence;
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
                    .AddQuartz(c =>
                    {
                        c.AddJob<JomashopDataSyncJob>(
                            o => o.WithIdentity(JomashopDataSyncJob.key));

                        var triggerName = $"{nameof(JomashopDataSyncJob)}-Trigger";

                        c.AddTrigger(o => o.ForJob(JomashopDataSyncJob.key)
                                           .WithIdentity(triggerName)
                                           .WithSimpleSchedule(b => b.WithIntervalInSeconds(10) //For app: .WithIntervalInMinutes(options.RunEveryMinutes)
                                                                     .RepeatForever()));
                    }).AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
        });

public sealed record WorkerOptions(int RunEveryMinutes)
{
    public const string SectionName = "WorkerOptions";
}

public sealed class JomashopDataSyncJob : IJob
{
    public static readonly JobKey key =
        new(nameof(JomashopDataSyncJob), "DataSync");

    public Task Execute(IJobExecutionContext context)
    {
        // TODO
        return Task.CompletedTask;
    }
}