using Dapper;
using JomashopNotifications;
using Microsoft.Data.SqlClient;

Console.WriteLine("Hello, World!");

// From Appsettings
const string ConnectionString = "Server=DESKTOP-5R95BQP;Database=Test;Integrated Security=True;TrustServerCertificate=True";

var jomashopLinksDatabase = new ProductLinksDatabase(ConnectionString);
var links = await jomashopLinksDatabase.ListProductLinksAsync();
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

public sealed record JomashopLink(int Id, string Link);

public sealed class ProductLinksDatabase(string ConnectionString)
{
    // Awaiting the function calls to ensure SqlConnection is properly used before disposal
    private const string TableName = "dbo.ProductLinks";

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

    public async Task<IEnumerable<JomashopLink>> ListProductLinksAsync()
    {
        using var connection = new SqlConnection(ConnectionString);

        var sql = $"SELECT * FROM {TableName} WITH(NOLOCK)";

        return await connection.QueryAsync<JomashopLink>(sql, CancellationToken.None);
    }
}