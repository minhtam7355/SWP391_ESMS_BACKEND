using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest emailRequest);
    }
}
