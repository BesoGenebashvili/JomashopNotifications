namespace JomashopNotifications.Persistence.Abstractions;

public interface IInStockProductsDatabase
{
    Task<int> InsertAsync(int productId, decimal price);
    Task<bool> DeleteAsync(int id);
}
