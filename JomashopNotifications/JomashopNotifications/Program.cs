using Dapper;
using JomashopNotifications;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var configuration = CreateConfiguration();
using var serviceProvider = CreateServiceProvider(configuration);
var productLinksDatabase = serviceProvider.GetRequiredService<IProductsDatabase>();

var links = await productLinksDatabase.ListProductsAsync();
var results = JomashopService.ParseProductsFromLinksAsync(links.Select(l => new Uri(l.Link)));

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

static ServiceProvider CreateServiceProvider(IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new InvalidOperationException("Missing ConnectionStrings.DefaultConnection in appsettings.json");

    return new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton<IProductsDatabase>(_ => new ProductsSqlDatabase(connectionString))
                .BuildServiceProvider();
}

public sealed record JomashopLink(int Id, string Link);

public interface IProductsDatabase
{
    Task<JomashopLink> GetJomashopLinkAsync(int id);
    Task<IEnumerable<JomashopLink>> ListProductsAsync();
}

public sealed class ProductsSqlDatabase(string ConnectionString) : IProductsDatabase
{
    // Awaiting the function calls to ensure SqlConnection is properly used before disposal
    private const string TableName = "dbo.Products";

    public async Task<JomashopLink> GetJomashopLinkAsync(int id)
    {
        using var connection = new SqlConnection(ConnectionString);

        var sql = $"SELECT * FROM {TableName} WITH(NOLOCK) WHERE Id = @Id";

        var @params = new
        {
            Id = id
        };

        return await connection.QuerySingleAsync<JomashopLink>(sql, @params);
    }

    public async Task<IEnumerable<JomashopLink>> ListProductsAsync()
    {
        using var connection = new SqlConnection(ConnectionString);

        var sql = $"SELECT * FROM {TableName} WITH(NOLOCK)";

        return await connection.QueryAsync<JomashopLink>(sql, CancellationToken.None);
    }
}