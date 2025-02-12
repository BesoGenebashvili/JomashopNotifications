using JomashopNotifications.Application;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Domain;
using JomashopNotifications.Persistence;
using JomashopNotifications.Persistence.Entities;
using MediatR;
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
                                           .WithSimpleSchedule(b => b.WithIntervalInSeconds(10) //For app: .WithIntervalInMinutes(options.RunEveryMinutes)
                                                                     .RepeatForever()));
                    }).AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
        });

public sealed record WorkerOptions(int RunEveryMinutes)
{
    public const string SectionName = "WorkerOptions";
}

public sealed class JomashopDataSyncJob(
    IMediator mediator,
    JomashopBrowserDriverService browserDriverService) : IJob
{
    public static readonly JobKey key =
        new(nameof(JomashopDataSyncJob), "DataSync");

    public async Task Execute(IJobExecutionContext context)
    {
        // List active products (I need to implement filters)
        var products = await mediator.Send(new ListProductsQuery());
        var activeProducts = products.Where(p => p.Status is ProductStatus.Active);

        if (!activeProducts.Any())
        {
            Log.Information("No active products found in the database");
            return;
        }

        Log.Debug("Active products: {@ProductIds}", string.Join(", ", activeProducts.Select(x => x.Id)));
        Log.Information("Found {Count} active products in the database.", activeProducts.Count());
    }
}