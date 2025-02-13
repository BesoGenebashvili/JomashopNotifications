using JomashopNotifications.Persistence.Entities;

namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductsDatabase
{
    Task<ProductEntity?> GetAsync(int id);
    Task<IEnumerable<ProductEntity>> ListAsync(ProductStatus? status);
    Task<int> InsertAsync(InsertProductEntity insertProductModel);
    Task<bool> SetStatusAsActiveAsync(int id);
    Task<bool> SetStatusAsInactiveAsync(int id);
    Task<bool> DeleteAsync(int id);
}
