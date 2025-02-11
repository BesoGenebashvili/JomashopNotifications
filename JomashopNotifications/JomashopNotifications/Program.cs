using JomashopNotifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JomashopNotifications.Persistence;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Domain;

Console.WriteLine("Hello, World!");

var configuration = CreateConfiguration();
using var serviceProvider = CreateServiceProvider(configuration);
var productsDatabase = serviceProvider.GetRequiredService<IProductsDatabase>();

var products = await productsDatabase.ListAsync();
var results = JomashopService.ParseProductsFromLinksAsync(products.Select(l => new Uri(l.Link)));

await foreach (var result in results)
{
    if (result.IsLeft(out var item))
    {
        Console.WriteLine(item);
    }

    if (result.IsRight(out var browserDriverError))
    {
        Console.WriteLine($"An error occured while operating with a browser: {browserDriverError}");
    }
}

Console.Read();

static IConfiguration CreateConfiguration() =>
    new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

static ServiceProvider CreateServiceProvider(IConfiguration configuration) =>
    new ServiceCollection()
            .AddSingleton(configuration)
            .AddPersistenceServices(configuration)
            .BuildServiceProvider();
