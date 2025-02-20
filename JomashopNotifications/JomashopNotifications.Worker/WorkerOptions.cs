namespace JomashopNotifications.Worker;

public sealed record JomashopDataSyncJobOptions(int RunEveryMinutes);
public sealed record InStockProductsPublisherJobOptions(int RunEveryMinutes);

public record WorkerOptions
{
    public const string SectionName = "WorkerOptions";

    public required JomashopDataSyncJobOptions JomashopDataSyncJobOptions { get; init; }
    public required InStockProductsPublisherJobOptions InStockProductsPublisherJobOptions { get; init; }
}