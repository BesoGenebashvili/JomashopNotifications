using JomashopNotifications.Persistence.Entities;

namespace JomashopNotifications.Persistence.Abstractions;

public interface IInStockProductsDatabase
{
    Task<IEnumerable<InStockProductEntity>> ListAsync();
    Task<int> UpsertAsync(int productId, decimal price, DateTime checkedAt);
    Task<bool> DeleteAsync(int id);
}
