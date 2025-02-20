using JomashopNotifications.Application;
using JomashopNotifications.Domain;
using JomashopNotifications.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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

            services.AddMassTransit(configuration)
                    .AddQuartz(configuration)
                    .AddPersistenceServices(configuration)
                    .AddApplicationServices()
                    .AddDomainServices();
        });