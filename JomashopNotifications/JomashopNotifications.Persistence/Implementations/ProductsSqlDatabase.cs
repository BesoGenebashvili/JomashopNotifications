using Dapper;
using JomashopNotifications.Domain.Common;
using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Common;
using JomashopNotifications.Persistence.Entities;
using Microsoft.Data.SqlClient;
using System.Text;

namespace JomashopNotifications.Persistence.Implementations;

// Awaiting the function calls to ensure SqlConnection is properly used before disposal
public sealed class ProductsSqlDatabase(string ConnectionString) : IProductsDatabase
{
    public async Task<ProductEntity?> GetAsync(int id)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new
        {
            id
        };

        var sql = $"""
                  SELECT
                        p.Id,
                        p.Brand,
                        p.Name,
                        p.Link,
                        p.Status,
                        p.CreatedAt,
                        p.UpdatedAt,
                        pi.IsPrimary,
                        pi.ImageData
                  FROM dbo.{DatabaseTable.Products} p WITH(NOLOCK)
                  LEFT JOIN dbo.{DatabaseTable.ProductImages} pi WITH(NOLOCK) ON p.Id = pi.ProductId
                  WHERE p.Id = @id
                  """;

        var products = await connection.QueryAsync<ProductEntity, ProductImageEntity, ProductEntity>(
                                            sql,
                                            (product, image) =>
                                            {
                                                product.Images.Add(image);
                                                return product;
                                            },
                                            @params,
                                            splitOn: "IsPrimary");

        return products.GroupBy(p => p.Id)
                       .Select(g =>
                       {
                           var groupedProduct = g.First();

                           groupedProduct = groupedProduct with
                           {
                               Images = g.Where(p => p.Images.SingleOrDefault() is not null)
                                         .Select(p => p.Images.Single())
                                         .ToList()
                           };

                           return groupedProduct;
                       })
                       .SingleOrDefault();
    }

    public async Task<IEnumerable<ProductEntity>> ListAsync(int[]? ids, ProductStatus? status)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new DynamicParameters();

        var sql = new StringBuilder($"SELECT * FROM dbo.{DatabaseTable.Products} WITH(NOLOCK) WHERE 1 = 1");

        if (status is { } statusValue)
        {
            sql.Append(" AND Status = @status");
            @params.Add("@status", statusValue);
        }

        if (ids.NullIfEmpty() is { } idsValue)
        {
            var uniqueIds = idsValue.Distinct();

            sql.Append(" AND Id IN @ids");
            @params.Add("@ids", uniqueIds);
        }

        return await connection.QueryAsync<ProductEntity>(sql.ToString(), @params);
    }

    public async Task<int> InsertAsync(InsertProductEntity insertProductModel)
    {
        using var connection = new SqlConnection(ConnectionString);

        var @params = new DynamicParameters(new
        {
            brand = insertProductModel.Brand,
            name = insertProductModel.Name,
            link = insertProductModel.Link,
            status = insertProductModel.Status,
            updatedAt = DateTime.UtcNow
        });

        @params.Add(
            "@id",
            dbType: System.Data.DbType.Int32,
            direction: System.Data.ParameterDirection.Output);

        var sql = $"""
                  INSERT INTO dbo.{DatabaseTable.Products} (Brand, Name, Link, Status, UpdatedAt)
                  VALUES (@brand, @name, @link, @status, @updatedAt)
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