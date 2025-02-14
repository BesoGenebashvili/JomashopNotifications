namespace JomashopNotifications.Persistence.Abstractions;

public interface IApplicationErrorsDatabase
{
    Task InsertAsync(string message, string? type);
}
