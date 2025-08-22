using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace hotel_management.Services.Mail
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;

        public MailService(MailSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task SendMailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass);

                var mail = new MailMessage
                {
                    From = new MailAddress(_settings.SmtpUser, "Hotel Management System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                try
                {
                    await client.SendMailAsync(mail);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error sending mail: " + ex);
                    throw;
                }
            }
        }
    }
}
