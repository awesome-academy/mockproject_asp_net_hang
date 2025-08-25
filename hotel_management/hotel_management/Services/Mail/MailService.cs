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
            try
            {
                using (var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort))
                {
                    //client.EnableSsl = true;
                    //client.Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass);

                    // TODO: Run on local without SSL
                    client.EnableSsl = false;
                    client.UseDefaultCredentials = true;

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(_settings.SmtpUser, "Hotel Management System");
                        mail.To.Add(toEmail);
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        await client.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error sending mail: " + ex);
                throw;
            }
        }
    }
}
