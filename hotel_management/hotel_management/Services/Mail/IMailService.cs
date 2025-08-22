using System.Threading.Tasks;

namespace hotel_management.Services.Mail
{
    public interface IMailService
    {
        Task SendMailAsync(string toEmail, string subject, string body);
    }
}
