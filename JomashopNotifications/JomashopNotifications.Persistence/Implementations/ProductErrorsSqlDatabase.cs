using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class ProductErrorsSqlDatabase(string ConnectionString) : IProductErrorsDatabase
{
    public async Task<int> InsertAsync(int productId, string message)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new DynamicParameters(new
        {
            productId,
            message,
            checkedAt = DateTime.UtcNow
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                  INSERT INTO dbo.{DatabaseTable.ProductErrors} (ProductId, Message, CheckedAt)
                  VALUES (@productId, @message, @checkedAt)
                  SET @id = SCOPE_IDENTITY();
                  """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    public Task<bool> DeleteAsync(int id) =>
        SqlDatabaseExtensions.DeleteFromTableAsync(
            ConnectionString, 
            DatabaseTable.ProductErrors, 
            id);
}