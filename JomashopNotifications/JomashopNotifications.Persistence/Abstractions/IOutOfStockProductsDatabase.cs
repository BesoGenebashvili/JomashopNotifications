namespace JomashopNotifications.Persistence.Abstractions;

public interface IOutOfStockProductsDatabase
{
    Task<int> UpsertAsync(int productId, DateTime checkedAt);
    Task<bool> DeleteAsync(int id);
}
