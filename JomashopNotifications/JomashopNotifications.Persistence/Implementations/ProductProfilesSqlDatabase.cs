using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using JomashopNotifications.Persistence.Entities.ProductProfile;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class ProductProfilesSqlDatabase(string ConnectionString) : IProductProfilesDatabase
{
    public async Task<IEnumerable<ProductProfileEntity>> ListAsync(int[]? productIds)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            productIds = productIds ?? []
        };

        var sql = $"""
                   SELECT * FROM dbo.{DatabaseTable.ProductProfiles} WITH(NOLOCK)
                   WHERE ProductId in @productIds
                   """;

        return await connection.QueryAsync<ProductProfileEntity>(sql.ToString(), @params);
    }

    public async Task<int> UpsertAsync(int productId, decimal priceThreshold)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new DynamicParameters(new
        {
            productId,
            priceThreshold
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                   IF EXISTS (SELECT 1 FROM dbo.{DatabaseTable.ProductProfiles} WHERE ProductId = @productId)
                       BEGIN
                           UPDATE dbo.{DatabaseTable.ProductProfiles} SET
                               PriceThreshold = @priceThreshold
                           WHERE ProductId = @productId;
                           SET @id = (SELECT Id FROM dbo.{DatabaseTable.ProductProfiles} WHERE ProductId = @productId);
                       END
                   ELSE
                       BEGIN
                           INSERT INTO dbo.{DatabaseTable.ProductProfiles} (ProductId, PriceThreshold)
                           VALUES (@productId, @priceThreshold);
                           SET @id = SCOPE_IDENTITY();
                       END
                   """;

        await connection.ExecuteAsync(sql, @params);

        return @params.Get<int>("@id");
    }

    private async Task<bool> SetIsActiveAsync(int id, bool isActive)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            id,
            isActive,
        };

        var sql = $"""
                   UPDATE dbo.{DatabaseTable.ProductProfiles} SET
                     IsActive = @isActive
                   WHERE Id = @id AND IsActive <> @isActive
                   """;

        return await connection.ExecuteAsync(sql, @params) > 0;
    }

    public Task<bool> ActivateAsync(int id) =>
        SetIsActiveAsync(id, true);

    public Task<bool> DeactivateAsync(int id) =>
        SetIsActiveAsync(id, false);

    public Task<bool> DeleteAsync(int id) =>
        SqlDatabaseExtensions.DeleteFromTableAsync(
            ConnectionString,
            DatabaseTable.ProductProfiles,
            id);

}
