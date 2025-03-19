namespace JomashopNotifications.EventHandler.EmailNotifications;

public sealed record EmailOptions
{
    public sealed record TemplateOptions
    {
        public required string Subject { get; init; }
        public required string BodyTemplateFilePath { get; init; }
        public bool IsBodyHtml { get; init; } = true;
    }

    public sealed record SenderEmailOptions
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public string DisplayName { get; init; } = "Jomashop Notification Sender";
    }

    public sealed record ReceiverEmailOptions
    {
        public required string Email { get; init; }
        public string DisplayName { get; init; } = "Jomashop Notification Receiver";
    }

    public required TemplateOptions Template { get; init; }
    public required SenderEmailOptions Sender { get; init; }
    public required ReceiverEmailOptions Receiver { get; init; }
    public required string Host { get; init; }
    public int Port { get; init; } = 587;
    public bool EnableSsl { get; init; } = true;
    public string[]? ReceiverBCCList { get; init; }
}
