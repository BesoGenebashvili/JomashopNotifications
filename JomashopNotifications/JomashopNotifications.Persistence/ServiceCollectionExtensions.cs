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

        services.AddScoped<IProductsDatabase>(_ => new ProductsSqlDatabase(connectionString));
        services.AddScoped<IInStockProductsDatabase>(_ => new InStockProductsSqlDatabase(connectionString));
        services.AddScoped<IOutOfStockProductsDatabase>(_ => new OutOfStockProductsSqlDatabase(connectionString));
        services.AddScoped<IProductParseErrorsDatabase>(_ => new ProductParseErrorsSqlDatabase(connectionString));
        services.AddScoped<IApplicationErrorsDatabase>(_ => new ApplicationErrorsSqlDatabase(connectionString));
        services.AddScoped<IProductProfilesDatabase>(_ => new ProductProfilesSqlDatabase(connectionString));

        return services;
    }
}
