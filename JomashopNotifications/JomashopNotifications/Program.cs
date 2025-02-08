using Dapper;
using JomashopNotifications;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var configuration = CreateConfiguration();
using var serviceProvider = CreateServiceProvider(configuration);
var productsDatabase = serviceProvider.GetRequiredService<IProductsDatabase>();

var links = await productsDatabase.ListProductsAsync();
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

public sealed record Product
{
    public int Id { get; init; }
    public required string Link { get; init; }
    public ProductStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public interface IProductsDatabase
{
    Task<Product> GetProductAsync(int id);
    Task<IEnumerable<Product>> ListProductsAsync();
    Task<int> InsertProductAsync(InsertProductModel insertProductModel);
    Task<bool> SetProductAsActiveAsync(int id);
    Task<bool> SetProductAsInactiveAsync(int id);
    Task<bool> DeleteProductAsync(int id);
}

//TODO: JomashopNotifications.Persistence

public enum ProductStatus : byte
{
    Active = 1,
    Inactive = 2
}

public sealed record InsertProductModel
{
    public required Uri Link { get; init; }
    public required ProductStatus Status { get; init; }
}

public sealed class ProductsSqlDatabase(string ConnectionString) : IProductsDatabase
{
    // Awaiting the function calls to ensure SqlConnection is properly used before disposal
    private const string TableName = "dbo.Products";

    public async Task<Product> GetProductAsync(int id)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            id
        };

        var sql = $"""
                  SELECT * FROM {TableName} WITH(NOLOCK) 
                  WHERE Id = @id
                  """;

        return await connection.QuerySingleAsync<Product>(sql, @params);
    }

    public async Task<IEnumerable<Product>> ListProductsAsync()
    {
        using var connection = new SqlConnection(ConnectionString);

        var sql = $"SELECT * FROM {TableName} WITH(NOLOCK)";

        return await connection.QueryAsync<Product>(sql, CancellationToken.None);
    }

    public async Task<int> InsertProductAsync(InsertProductModel insertProductModel)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new DynamicParameters(new
        {
            link = insertProductModel.Link.AbsoluteUri,
            status = insertProductModel.Status,
            updatedAt = DateTime.UtcNow
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                  INSERT INTO {TableName} (Link, Status, UpdatedAt)
                  VALUES (@link, @status, @updatedAt)
                  SET @id = SCOPE_IDENTITY();
                  """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    private async Task<bool> SetProductStatusAsync(int id, ProductStatus status)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            id,
            status,
            updatedAt = DateTime.UtcNow
        };

        var sql = $"""
                  UPDATE {TableName} SET
                    Status = @status,
                    UpdatedAt = @updatedAt
                  WHERE Id = @id AND Status <> @status
                  """;

        return await connection.ExecuteAsync(sql, @params) > 0;
    }

    public Task<bool> SetProductAsActiveAsync(int id) =>
        SetProductStatusAsync(id, ProductStatus.Active);

    public Task<bool> SetProductAsInactiveAsync(int id) =>
        SetProductStatusAsync(id, ProductStatus.Inactive);

    public async Task<bool> DeleteProductAsync(int id)
    {
        var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            id
        };

        var sql = $"""
                   DELETE FROM {TableName}
                   WHERE Id = @id
                   """;

        return await connection.ExecuteAsync(sql, @params) > 0;
    }
}