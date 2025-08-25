using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace hotel_management.Services.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public static AuthResult Ok() => new AuthResult { Success = true };
        public static AuthResult Fail(string msg) => new AuthResult { Success = false, Error = msg };
    }

    public interface IAuthService
    {
        AuthResult Login(string emailOrUsername, string password);
        void Logout();

        Task<AuthResult> SendPasswordResetAsync(string email, string baseUrl);

        AuthResult ResetPassword(string token, string newPassword);
    }
}
