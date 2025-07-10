using GameIt.Application.Models.Email;

namespace GameIt.Application.Interfaces.Email;

public interface IEmailSender
{
    Task<bool> SendEmail(EmailMessage email);
}
