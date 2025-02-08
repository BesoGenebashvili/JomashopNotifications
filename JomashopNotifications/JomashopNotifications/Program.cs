using Dapper;
using JomashopNotifications;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var configuration = CreateConfiguration();
using var serviceProvider = CreateServiceProvider(configuration);
var productLinksDatabase = serviceProvider.GetRequiredService<IProductDatabase>();

var links = await productLinksDatabase.ListProductAsync();
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
                .AddSingleton<IProductDatabase>(_ => new ProductSqlDatabase(connectionString))
                .BuildServiceProvider();
}

public sealed record JomashopLink(int Id, string Link);

public interface IProductDatabase
{
    Task<JomashopLink> GetJomashopLinkAsync(int id);
    Task<IEnumerable<JomashopLink>> ListProductAsync();
}

public sealed class ProductSqlDatabase(string ConnectionString) : IProductDatabase
{
    // Awaiting the function calls to ensure SqlConnection is properly used before disposal
    private const string TableName = "dbo.Product";

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

    public async Task<IEnumerable<JomashopLink>> ListProductAsync()
    {
        using var connection = new SqlConnection(ConnectionString);

        var sql = $"SELECT * FROM {TableName} WITH(NOLOCK)";

        return await connection.QueryAsync<JomashopLink>(sql, CancellationToken.None);
    }
}