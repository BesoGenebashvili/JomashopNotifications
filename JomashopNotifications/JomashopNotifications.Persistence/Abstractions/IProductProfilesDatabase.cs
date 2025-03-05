using JomashopNotifications.Persistence.Entities.ProductProfile;

namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductProfilesDatabase
{
    Task<IEnumerable<ProductProfileEntity>> ListAsync(int[]? productIds);
    Task<int> UpsertAsync(int productId, decimal priceThreshold);
    Task<bool> ActivateAsync(int id);
    Task<bool> DeactivateAsync(int id);
    Task<bool> DeleteAsync(int id);
}
