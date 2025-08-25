using hotel_management.Models;
using hotel_management.Models.AuthView;
using System.Threading.Tasks;

namespace hotel_management.Services.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public static AuthResult Ok () => new AuthResult { Success = true };
        public static AuthResult Fail (string msg) => new AuthResult { Success = false, Error = msg };
    }

    public interface IAuthService
    {
        Task<bool> IsEmailValidAsync(string email);

        Task<AuthResult> RegisterUser(RegisterViewModel model, string baseUrl);

        AuthResult ActivateUser(string token);

        Task<AuthResult> ResendActivationEmail(string email, string baseUrl);

        User GetUserByToken(string token);
    }
}
