namespace JomashopNotifications.EventHandler.EmailNotifications;

public sealed record EmailOptions
{
    public sealed record SenderEmailOptions
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public string DisplayName { get; init; } = "Jomashop Notifications";
    }

    public sealed record ReceiverEmailOptions
    {
        public required string Email { get; init; }
        public string DisplayName { get; init; } = "Jomashop Notifications Receiver";
    }

    public required SenderEmailOptions Sender { get; init; }
    public required ReceiverEmailOptions Receiver { get; init; }
    public required string Host { get; init; }
    public int Port { get; init; } = 587;
    public bool EnableSsl { get; init; } = true;
    public string[]? ReceiverBCCList { get; init; }
}
