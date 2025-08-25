using dotenv.net;
using hotel_management.Services.Mail;
using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Unity;
using Unity.AspNet.Mvc;

namespace hotel_management
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var rootPath = Server.MapPath("~/");

            DotEnv.Load(new DotEnvOptions(
                envFilePaths: new[] { System.IO.Path.Combine(rootPath, ".env") },
                probeForEnv: false
            ));

            var mailSettings = new MailSettings
            {
                SmtpUser = Environment.GetEnvironmentVariable("SMTP_USER"),
                SmtpPass = Environment.GetEnvironmentVariable("SMTP_PASS"),
                SmtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com",
                SmtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587")
            };

            var container = new UnityContainer();
            container.RegisterInstance(mailSettings);
            container.RegisterType<IMailService, MailService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                return;
            }

            try
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket == null || authTicket.Expired)
                {
                    return;
                }
                string[] roles = { authTicket.UserData };
                var principal = new GenericPrincipal(new FormsIdentity(authTicket), roles);
                Context.User = principal;
            }
            catch (Exception)
            {
                FormsAuthentication.SignOut();
                Response.Redirect(FormsAuthentication.LoginUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
