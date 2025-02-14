using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class InStockProductsSqlDatabase(string ConnectionString) : IInStockProductsDatabase
{
    public async Task<int> UpsertAsync(
        int productId, 
        decimal price,
        DateTime checkedAt)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new DynamicParameters(new
        {
            productId,
            price,
            checkedAt
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                   IF EXISTS (SELECT 1 FROM dbo.{DatabaseTable.InStockProducts} WHERE ProductId = @productId)
                       BEGIN
                           UPDATE dbo.{DatabaseTable.InStockProducts} SET
                               Price = @price, 
                               CheckedAt = @checkedAt
                           WHERE ProductId = @productId;
                           SET @id = (SELECT Id FROM dbo.{DatabaseTable.InStockProducts} WHERE ProductId = @productId);
                       END
                   ELSE
                       BEGIN
                           INSERT INTO dbo.{DatabaseTable.InStockProducts} (ProductId, Price, CheckedAt)
                           VALUES (@productId, @price, @checkedAt);
                           SET @id = SCOPE_IDENTITY();
                       END
                   """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    public Task<bool> DeleteAsync(int id) =>
        SqlDatabaseExtensions.DeleteFromTableAsync(
            ConnectionString,
            DatabaseTable.InStockProducts,
            id);
}
