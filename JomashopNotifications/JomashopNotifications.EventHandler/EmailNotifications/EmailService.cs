using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace JomashopNotifications.EventHandler.EmailNotifications;

public class EmailService(IOptions<EmailOptions> emailOptions)
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task SendAsync(Dictionary<EmailTemplatePlaceholderKey, string> emailTemplatePlaceholders)
    {
        var templateFilePath = _emailOptions.Template.BodyTemplateFilePath
                                             ?? throw new ApplicationException("Email template file path is not provided in appsettings.json");

        var template = await File.ReadAllTextAsync(templateFilePath);

        if (string.IsNullOrWhiteSpace(template))
            throw new ApplicationException($"Email template file '{templateFilePath}' is empty");

        var body = emailTemplatePlaceholders.Aggregate(
                        template,
                        (current, placeholder) => current.Replace($"{{{placeholder.Key}}}", placeholder.Value));

        await SendAsync(body);
    }

    private Task SendAsync(string body)
    {
        var fromMailAddress = new MailAddress(_emailOptions.Sender.Email, _emailOptions.Sender.DisplayName);
        var fromCredentials = new NetworkCredential(_emailOptions.Sender.Email, _emailOptions.Sender.Password);

        var toMailAddress = new MailAddress(_emailOptions.Receiver.Email, _emailOptions.Receiver.DisplayName);

        using var smtpClient = new SmtpClient
        {
            Host = _emailOptions.Host,
            Port = _emailOptions.Port,
            EnableSsl = _emailOptions.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = fromCredentials
        };

        using var mailMessage = new MailMessage(fromMailAddress, toMailAddress)
        {
            IsBodyHtml = _emailOptions.Template.IsBodyHtml,
            Subject = _emailOptions.Template.Subject,
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
