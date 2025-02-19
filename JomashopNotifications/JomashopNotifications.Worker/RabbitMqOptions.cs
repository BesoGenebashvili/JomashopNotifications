namespace JomashopNotifications.Worker;

public sealed record RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public required string Host { get; init; }
    public required ushort Port { get; init; }
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
}