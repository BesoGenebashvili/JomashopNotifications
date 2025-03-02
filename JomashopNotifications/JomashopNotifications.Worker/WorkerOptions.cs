namespace JomashopNotifications.Worker;

public sealed record JomashopDataSyncJobOptions(bool IsActive, int RunEveryMinutes);
public sealed record InStockProductsPublisherJobOptions(bool IsActive, int RunEveryMinutes);

public record WorkerOptions
{
    public const string SectionName = "WorkerOptions";

    public required JomashopDataSyncJobOptions JomashopDataSyncJobOptions { get; init; }
    public required InStockProductsPublisherJobOptions InStockProductsPublisherJobOptions { get; init; }
}