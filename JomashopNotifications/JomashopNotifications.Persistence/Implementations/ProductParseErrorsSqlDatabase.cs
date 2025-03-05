using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class ProductParseErrorsSqlDatabase(string connectionString) : IProductParseErrorsDatabase
{
    public async Task<int> UpsertAsync(int productId, string message, DateTime checkedAt)
    {
        using var connection = new SqlConnection(connectionString);

        var @params = new DynamicParameters(new
        {
            productId,
            message,
            checkedAt
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                   IF EXISTS (SELECT 1 FROM dbo.{DatabaseTable.ProductParseErrors} WHERE ProductId = @productId)
                       BEGIN
                           UPDATE dbo.{DatabaseTable.ProductParseErrors} SET
                               Message = @message, 
                               CheckedAt = @checkedAt
                           WHERE ProductId = @productId;
                           SET @id = (SELECT Id FROM dbo.{DatabaseTable.ProductParseErrors} WHERE ProductId = @productId);
                       END
                   ELSE
                       BEGIN
                           INSERT INTO dbo.{DatabaseTable.ProductParseErrors} (ProductId, Message, CheckedAt)
                           VALUES (@productId, @message, @checkedAt);
                           SET @id = SCOPE_IDENTITY();
                       END
                   """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    public Task<bool> DeleteAsync(int id) =>
        SqlDatabaseExtensions.DeleteFromTableAsync(
            connectionString, 
            DatabaseTable.ProductParseErrors, 
            id);
}