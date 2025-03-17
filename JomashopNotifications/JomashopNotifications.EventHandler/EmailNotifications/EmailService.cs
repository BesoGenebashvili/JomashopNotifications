using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace JomashopNotifications.EventHandler.EmailNotifications;

public class EmailService(IOptions<EmailOptions> emailOptions)
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public Task SendAsync(
        string recipientMailAddress,
        string recipientDisplayName,
        string subject,
        string body,
        bool isBodyHtml = true)
    {
        var fromMailAddress = new MailAddress(_emailOptions.Sender.Email, _emailOptions.Sender.DisplayName);
        var fromCredentials = new NetworkCredential(_emailOptions.Sender.Email, _emailOptions.Sender.Password);

        var toMailAddress = new MailAddress(recipientMailAddress, recipientDisplayName);

        using var smtpClient = new SmtpClient
        {
            Host = _emailOptions.Host,
            Port = _emailOptions.Port,
            EnableSsl = _emailOptions.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = fromCredentials
        };

        using MailMessage mailMessage = new(fromMailAddress, toMailAddress)
        {
            IsBodyHtml = isBodyHtml,
            Subject = subject,
            Body = body
        };

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        _emailOptions.ReceiverBCCList?
                     .Select(address => MailAddress.TryCreate(address, out var result) ? result : null)
                     .Where(mailAddress => mailAddress is not null)
                     .ToList()
                     .ForEach(mailMessage.Bcc.Add);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

        return smtpClient.SendMailAsync(mailMessage);
    }
}
