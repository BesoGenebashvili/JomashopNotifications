using JomashopNotifications.Persistence.Entities.Product;

namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductsDatabase
{
    Task<ProductEntity?> GetAsync(int id);
    Task<IEnumerable<ProductEntity>> ListAsync(int[]? ids, ProductStatus? status);
    Task<int> InsertAsync(InsertProductEntity insertProductModel);
    Task<bool> ActivateAsync(int id);
    Task<bool> DeactivateAsync(int id);
    Task<bool> DeleteAsync(int id);
}
