using JomashopNotifications.Persistence.Entities.ProductProfile;

namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductProfilesDatabase
{
    Task<IEnumerable<ProductProfileEntity>> ListAsync(int[]? productIds);
    Task<int> UpsertAsync(int productId, decimal priceThreshold);
    Task<bool> ActivateAsync(int productId);
    Task<bool> DeactivateAsync(int productId);
    Task<bool> DeleteAsync(int productId);
}
