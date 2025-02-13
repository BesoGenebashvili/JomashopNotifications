using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JomashopNotifications.Persistence;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Domain;
using JomashopNotifications.Domain.Models;
using JomashopNotifications.Persistence.Entities;

Console.WriteLine("Hello, World!");

var configuration = CreateConfiguration();
using var serviceProvider = CreateServiceProvider(configuration);
var productsDatabase = serviceProvider.GetRequiredService<IProductsDatabase>();
var jomashopBrowserDriverService = serviceProvider.GetRequiredService<JomashopBrowserDriverService>();

var products = await productsDatabase.ListAsync(ProductStatus.Active);

var productsToCheck = products.Where(p => p.Status is ProductStatus.Active && Uri.IsWellFormedUriString(p.Link, UriKind.Absolute))
                              .Select(p => new Product.ToBeChecked(p.Id, new(p.Link))).Take(2)
                              .ToList();

var results = await jomashopBrowserDriverService.CheckProductsAsync(productsToCheck);

foreach (var result in results)
{
    if (result.IsLeft(out var item))
    {
        Console.WriteLine(item.Show());
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
            .AddDomainServices()
            .BuildServiceProvider();
