namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductParseErrorsDatabase
{
    Task<int> UpsertAsync(int productId, string message, DateTime checkedAt);
    Task<bool> DeleteAsync(int id);
}