namespace JomashopNotifications.Persistence.Abstractions;

public interface IOutOfStockProductsDatabase
{
    Task<int> InsertAsync(int productId);
    Task<bool> DeleteAsync(int id);
}
