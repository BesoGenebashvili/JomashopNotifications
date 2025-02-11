using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JomashopNotifications.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSqlDatabase(configuration);

    private static IServiceCollection AddSqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new InvalidOperationException("Missing ConnectionStrings.DefaultConnection in appsettings.json");

        services.AddSingleton<IProductsDatabase>(_ => new ProductsSqlDatabase(connectionString));
        services.AddSingleton<IInStockProductsDatabase>(_ => new InStockProductsSqlDatabase(connectionString));
        services.AddSingleton<IOutOfStockProductsDatabase>(_ => new OutOfStockProductsSqlDatabase(connectionString));
        services.AddSingleton<IProductErrorsDatabase>(_ => new ProductErrorsSqlDatabase(connectionString));

        return services;
    }
}
