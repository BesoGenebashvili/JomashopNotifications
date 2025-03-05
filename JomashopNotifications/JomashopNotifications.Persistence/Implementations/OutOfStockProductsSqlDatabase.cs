using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class OutOfStockProductsSqlDatabase(string connectionString) : IOutOfStockProductsDatabase
{
    public async Task<int> UpsertAsync(int productId, DateTime checkedAt)
    {
        using var connection = new SqlConnection(connectionString);

        var @params = new DynamicParameters(new
        {
            productId,
            checkedAt
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                   IF EXISTS (SELECT 1 FROM dbo.{DatabaseTable.OutOfStockProducts} WHERE ProductId = @productId)
                       BEGIN
                           UPDATE dbo.{DatabaseTable.OutOfStockProducts} SET
                               CheckedAt = @checkedAt
                           WHERE ProductId = @productId;
                           SET @id = (SELECT Id FROM dbo.{DatabaseTable.OutOfStockProducts} WHERE ProductId = @productId);
                       END
                   ELSE
                       BEGIN
                           INSERT INTO dbo.{DatabaseTable.OutOfStockProducts} (ProductId, CheckedAt)
                           VALUES (@productId, @checkedAt);
                           SET @id = SCOPE_IDENTITY();
                       END
                   """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    public Task<bool> DeleteAsync(int id) =>
        SqlDatabaseExtensions.DeleteFromTableAsync(
            connectionString,
            DatabaseTable.OutOfStockProducts,
            id);
}
