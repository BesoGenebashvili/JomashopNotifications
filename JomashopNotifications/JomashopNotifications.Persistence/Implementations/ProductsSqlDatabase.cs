using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using JomashopNotifications.Persistence.Entities;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

// Awaiting the function calls to ensure SqlConnection is properly used before disposal
public sealed class ProductsSqlDatabase(string ConnectionString) : IProductsDatabase
{
    public async Task<Product> GetAsync(int id)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            id
        };

        var sql = $"""
                  SELECT * FROM dbo.{DatabaseTable.Products} WITH(NOLOCK) 
                  WHERE Id = @id
                  """;

        return await connection.QuerySingleAsync<Product>(sql, @params);
    }

    public async Task<IEnumerable<Product>> ListAsync()
    {
        using var connection = new SqlConnection(ConnectionString);

        var sql = $"SELECT * FROM dbo.{DatabaseTable.Products} WITH(NOLOCK)";

        return await connection.QueryAsync<Product>(sql, CancellationToken.None);
    }

    public async Task<int> InsertAsync(InsertProductModel insertProductModel)
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
                  INSERT INTO dbo.{DatabaseTable.Products} (Link, Status, UpdatedAt)
                  VALUES (@link, @status, @updatedAt)
                  SET @id = SCOPE_IDENTITY();
                  """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    private async Task<bool> SetStatusAsync(int id, ProductStatus status)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            id,
            status,
            updatedAt = DateTime.UtcNow
        };

        var sql = $"""
                  UPDATE dbo.{DatabaseTable.Products} SET
                    Status = @status,
                    UpdatedAt = @updatedAt
                  WHERE Id = @id AND Status <> @status
                  """;

        return await connection.ExecuteAsync(sql, @params) > 0;
    }

    public Task<bool> SetStatusAsActiveAsync(int id) =>
        SetStatusAsync(id, ProductStatus.Active);

    public Task<bool> SetStatusAsInactiveAsync(int id) =>
        SetStatusAsync(id, ProductStatus.Inactive);

    public Task<bool> DeleteAsync(int id) =>
        SqlDatabaseExtensions.DeleteFromTableAsync(
            ConnectionString,
            DatabaseTable.Products,
            id);
}