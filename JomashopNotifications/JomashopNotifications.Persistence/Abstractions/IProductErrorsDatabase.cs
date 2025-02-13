namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductErrorsDatabase
{
    Task<int> UpsertAsync(int productId, string message);
    Task<bool> DeleteAsync(int id);
}