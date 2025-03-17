namespace JomashopNotifications.EventHandler.EmailNotifications;

public sealed record EmailOptions
{
    public const string SectionName = "EmailOptions";

    public required string SMTPFrom { get; set; }
    public required string Password { get; set; }
    public string DisplayName { get; set; } = "Jomashop Notifications";
    public required string Host { get; set; }
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string[]? BCCRecipientList { get; set; }
}
