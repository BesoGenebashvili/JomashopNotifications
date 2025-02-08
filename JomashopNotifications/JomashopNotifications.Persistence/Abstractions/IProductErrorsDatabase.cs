namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductErrorsDatabase
{
    Task<int> InsertAsync(int productId, string message);
    Task<bool> DeleteAsync(int id);
}