using GameIt.Application.Interfaces.Email;
using GameIt.Application.Models.Email;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GameIt.Infrastructure.EmailService;

public class EmailSender : IEmailSender
{
    public EmailSettings _emailSettings { get; }
    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    async Task<bool> IEmailSender.SendEmail(EmailMessage email)
    {
        var client = new SendGridClient(_emailSettings.ApiKey);
        var to = new EmailAddress(email.To);
        var from = new EmailAddress
        {
            Email = _emailSettings.FromAddress,
            Name = _emailSettings.FromName
        };

        var msg = MailHelper.CreateSingleEmail(from, to, email.Subject, email.Body, email.Body);

        var response = await client.SendEmailAsync(msg);

        return response.IsSuccessStatusCode;
    }
}
