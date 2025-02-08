using JomashopNotifications.Persistence.Entities;

namespace JomashopNotifications.Persistence.Abstractions;

public interface IProductsDatabase
{
    Task<Product> GetAsync(int id);
    Task<IEnumerable<Product>> ListAsync();
    Task<int> InsertAsync(InsertProductModel insertProductModel);
    Task<bool> SetStatusAsActiveAsync(int id);
    Task<bool> SetStatusAsInactiveAsync(int id);
    Task<bool> DeleteAsync(int id);
}
