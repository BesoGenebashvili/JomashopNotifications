using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace JomashopNotifications.EventHandler.EmailNotifications;

public class MailModel
{
    public string EmailAddress { get; set; }
    public string DisplayName { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsBodyHtml { get; set; }
}

public class EmailService(IOptions<EmailOptions> emailOptions)
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task<bool> SendAsync(MailModel mailModel)
    {
        try
        {
            var fromMailAddress = new MailAddress(_emailOptions.SMTPFrom, _emailOptions.DisplayName);
            var fromCredentials = new NetworkCredential(_emailOptions.SMTPFrom, _emailOptions.Password);

            var toMailAddress = new MailAddress(mailModel.EmailAddress, mailModel.DisplayName);

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
                IsBodyHtml = mailModel.IsBodyHtml,
                Subject = mailModel.Subject,
                Body = mailModel.Body,
            };

            // TODO
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            _emailOptions.BCCRecipientList?
                         .Select(address => MailAddress.TryCreate(address, out var result) ? result : null)
                         .Where(mailAddress => mailAddress is not null)
                         .ToList()
                         .ForEach(mailMessage.Bcc.Add);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            await smtpClient.SendMailAsync(mailMessage);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
