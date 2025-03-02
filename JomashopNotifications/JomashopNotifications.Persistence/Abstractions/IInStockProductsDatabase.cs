using JomashopNotifications.Persistence.Entities.InStockProduct;
using JomashopNotifications.Persistence.Entities.Product;

namespace JomashopNotifications.Persistence.Abstractions;

public interface IInStockProductsDatabase
{
    Task<IEnumerable<InStockProductEntity>> ListAsync();
    Task<int> UpsertAsync(UpsertInStockProductEntity productEntity);
    Task<bool> DeleteAsync(int id);
}
