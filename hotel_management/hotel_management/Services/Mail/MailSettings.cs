using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hotel_management.Services.Mail
{
    public class MailSettings
    {
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
    }
}
