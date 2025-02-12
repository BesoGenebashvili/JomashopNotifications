using JomashopNotifications.Application;
using JomashopNotifications.Application.InStockProduct.Commands;
using JomashopNotifications.Application.OutOfStockProduct.Commands;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Application.ProductError.Commands;
using JomashopNotifications.Domain;
using JomashopNotifications.Domain.Models;
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
                                           .WithSimpleSchedule(b => b.WithIntervalInMinutes(1) //For app: .WithIntervalInMinutes(options.RunEveryMinutes)
                                                                     .RepeatForever()));
                    }).AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
        });

public sealed record WorkerOptions(int RunEveryMinutes)
{
    public const string SectionName = "WorkerOptions";
}

[DisallowConcurrentExecution]
public sealed class JomashopDataSyncJob(
    IMediator mediator,
    JomashopBrowserDriverService browserDriverService) : IJob
{
    public static readonly JobKey key =
        new(nameof(JomashopDataSyncJob), "DataSync");

    public async Task Execute(IJobExecutionContext context)
    {
        // List active products (I need to implement filters)
        var allProducts = await mediator.Send(new ListProductsQuery());
        var activeProducts = allProducts.Where(p => p.Status is ProductStatus.Active);

        if (!activeProducts.Any())
        {
            Log.Information("No active products found in the database");
            return;
        }

        Log.Debug("Active products: {ProductIds}", string.Join(", ", activeProducts.Select(x => x.Id)));
        Log.Information("Found {Count} active products in the database.", activeProducts.Count());

        var activeProductsWithInvalidLink = activeProducts.Where(p => !Uri.IsWellFormedUriString(p.Link, UriKind.Absolute));

        if (activeProductsWithInvalidLink.Any())
        {
            Log.Warning(
                "Found {Count} products with invalid links: {ProductIds}",
                activeProductsWithInvalidLink.Count(),
                string.Join(", ", activeProductsWithInvalidLink.Select(x => x.Id)));

            activeProducts = activeProducts.Except(activeProductsWithInvalidLink);
        }

        var productsToCheck = activeProducts.Select(p => new Product.ToBeChecked(new(p.Link)));

        var productCheckResults = await browserDriverService.CheckProductsAsync(productsToCheck);

        // Add Id to Product.ToBeChecked ?
        var idsAndCheckResults = activeProducts.Select(p => p.Id)
                                               .Zip(productCheckResults,
                                                    (Id, CheckResult) => (Id, CheckResult)); // Just for naming

        // SplitAt? Partition?
        var browserDriverErrors = idsAndCheckResults.Where(x => x.CheckResult.IsRight(out _));

        if (browserDriverErrors.Any())
        {
            Log.Error(
                "An error occurred while operating with the browser for products: {ProductIds}",
                browserDriverErrors.Select(e => e.Id));
        }

        var idsAndValidCheckedResults = idsAndCheckResults.Except(browserDriverErrors)//                       this will never happen - I need better way to handle this
                                                          .Select(t => (t.Id, Checked: t.CheckResult.Match(c => c, _ => throw new())));

        Log.Debug("Successfully checked product results: {}", idsAndValidCheckedResults.Select(r => $"{r.Id} - {r.Checked.Show()}"));
        Log.Information("Successfully checked products: {ProductIds}", idsAndValidCheckedResults);

        var commands = idsAndValidCheckedResults.Select(
                            x => mediator.Send(
                                    ResolveUpsertCommand(x.Id, x.Checked)));

        try
        {
            await Task.WhenAll(commands);
            Log.Information("Successfully upserted products: {ProductIds}", idsAndValidCheckedResults.Select(x => x.Id));
        }
        catch (Exception ex)
        {
            Log.Error("An error occurred while upserting products: {ProductIds}, Error: {ex}", idsAndValidCheckedResults.Select(x => x.Id), ex);
        }

        static IRequest<int> ResolveUpsertCommand(int id, Product.Checked @checked) =>
            @checked switch
            {
                Product.Checked.InStock(_, var price) => new UpsertInStockProductCommand(id, price.Value),
                Product.Checked.OutOfStock(_) => new UpsertOutOfStockProductCommand(id),
                Product.Checked.Error(_, var message) => new UpsertProductErrorCommand(id, message),
                _ => throw new NotImplementedException(nameof(Product.Checked))
            };
    }
}