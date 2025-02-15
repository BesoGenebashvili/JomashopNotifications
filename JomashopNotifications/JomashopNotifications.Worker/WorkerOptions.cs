namespace JomashopNotifications.Worker;

public sealed record WorkerOptions(int RunEveryMinutes)
{
    public const string SectionName = "WorkerOptions";
}