using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class OutOfStockProductsSqlDatabase(string ConnectionString) : IOutOfStockProductsDatabase
{
    public async Task<int> InsertAsync(int productId)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new DynamicParameters(new
        {
            productId,
            checkedAt = DateTime.UtcNow
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                  INSERT INTO dbo.{DatabaseTable.OutOfStockProducts} (ProductId, CheckedAt)
                  VALUES (@productId, @checkedAt)
                  SET @id = SCOPE_IDENTITY();
                  """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    public Task<bool> DeleteAsync(int id) =>
        SqlDatabaseExtensions.DeleteFromTableAsync(
            ConnectionString,
            DatabaseTable.OutOfStockProducts,
            id);
}
