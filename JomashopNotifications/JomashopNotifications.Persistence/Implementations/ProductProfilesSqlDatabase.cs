using Dapper;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using JomashopNotifications.Persistence.Entities.ProductProfile;
using Microsoft.Data.SqlClient;

namespace JomashopNotifications.Persistence.Implementations;

public sealed class ProductProfilesSqlDatabase(string connectionString) : IProductProfilesDatabase
{
    public async Task<IEnumerable<ProductProfileEntity>> ListAsync(int[]? productIds)
    {
        using var connection = new SqlConnection(connectionString);

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

    public async Task UpsertAsync(int productId, decimal priceThreshold)
    {
        using var connection = new SqlConnection(connectionString);

        var @params = new
        {
            productId,
            priceThreshold
        };

        var sql = $"""
                   IF EXISTS (SELECT 1 FROM dbo.{DatabaseTable.ProductProfiles} WHERE ProductId = @productId)
                       BEGIN
                           UPDATE dbo.{DatabaseTable.ProductProfiles} SET
                               PriceThreshold = @priceThreshold
                           WHERE ProductId = @productId;
                       END
                   ELSE
                       BEGIN
                           INSERT INTO dbo.{DatabaseTable.ProductProfiles} (ProductId, PriceThreshold)
                           VALUES (@productId, @priceThreshold);
                       END
                   """;

        await connection.ExecuteAsync(sql, @params);
    }

    private async Task<bool> SetIsActiveAsync(int productId, bool isActive)
    {
        using var connection = new SqlConnection(connectionString);

        var @params = new
        {
            productId,
            isActive,
        };

        var sql = $"""
                   UPDATE dbo.{DatabaseTable.ProductProfiles} SET
                     IsActive = @isActive
                   WHERE ProductId = @productId AND IsActive <> @isActive
                   """;

        return await connection.ExecuteAsync(sql, @params) > 0;
    }

    public Task<bool> ActivateAsync(int productId) =>
        SetIsActiveAsync(productId, true);

    public Task<bool> DeactivateAsync(int productId) =>
        SetIsActiveAsync(productId, false);

    public async Task<bool> DeleteAsync(int productId)
    {
        var connection = new SqlConnection(connectionString);

        var @params = new
        {
            productId
        };

        var sql = $"""
                   DELETE FROM dbo.{DatabaseTable.ProductProfiles}
                   WHERE ProductId = @productId;
                   """;

        return await connection.ExecuteAsync(sql, @params) > 0;
    }
}
