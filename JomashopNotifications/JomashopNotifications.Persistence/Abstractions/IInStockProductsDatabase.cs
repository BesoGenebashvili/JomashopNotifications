namespace JomashopNotifications.Persistence.Abstractions;

public interface IInStockProductsDatabase
{
    Task<int> UpsertAsync(int productId, decimal price, DateTime checkedAt);
    Task<bool> DeleteAsync(int id);
}
