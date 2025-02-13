namespace JomashopNotifications.Persistence.Abstractions;

public interface IOutOfStockProductsDatabase
{
    Task<int> UpsertAsync(int productId);
    Task<bool> DeleteAsync(int id);
}
