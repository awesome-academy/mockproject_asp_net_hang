using hotel_management.Models;
using hotel_management.Services.Mail;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using hotel_management.Enums;

namespace hotel_management.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IMailService _mail;

        public AuthService(AppDbContext db, IMailService mail)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mail = mail ?? throw new ArgumentNullException(nameof(mail));
        }

        public AuthResult Login(string emailOrUsername, string password)
        {
            if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrWhiteSpace(password))
                return AuthResult.Fail("Thiếu thông tin đăng nhập.");

            var user = _db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

            if (user == null)
                return AuthResult.Fail("Tài khoản không tồn tại.");

            if (!PasswordHasher.Verify(password, user.Password))
                return AuthResult.Fail("Mật khẩu không đúng.");

            if (!user.IsActive)
                return AuthResult.Fail("Tài khoản chưa được kích hoạt.");

            var ticket = new FormsAuthenticationTicket(
                1,
                emailOrUsername,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                user.Role.Name,
                FormsAuthentication.FormsCookiePath
            );
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            HttpContext.Current.Response.Cookies.Add(authCookie);
            return AuthResult.Ok();
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }

        public async Task<AuthResult> SendPasswordResetAsync(string email, string baseUrl)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return AuthResult.Ok();

            var token = GenerateToken();
            var act = new UserActivation
            {
                UserId = user.Id,
                Token = token,
                TokenType = TokenType.PasswordReset,
                ExpiredAt = DateTime.UtcNow.AddMinutes(30),
                IsUsed = TokenStatus.Unused,
                CreatedAt = DateTime.UtcNow
            };
            _db.UserActivations.Add(act);
            await _db.SaveChangesAsync();

            var link = $"{baseUrl}/Auth/ResetPassword?token={Uri.EscapeDataString(token)}";
            var subject = "Đặt lại mật khẩu Hotel Management";
            var body = $@"
                <p>Chào {user.Username ?? user.Email},</p>
                <p>Bạn vừa yêu cầu đặt lại mật khẩu.</p>
                <p>Nhấn vào liên kết sau để đặt lại (hết hạn trong 30 phút):</p>
                <p><a href=""{link}"">{link}</a></p>";

            await _mail.SendMailAsync(user.Email, subject, body);
            return AuthResult.Ok();
        }

        public AuthResult ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return AuthResult.Fail("Mật khẩu mới không hợp lệ.");

            var now = DateTime.UtcNow;
            var act = _db.UserActivations.FirstOrDefault(a =>
                a.Token == token &&
                a.TokenType == TokenType.PasswordReset &&
                a.IsUsed == TokenStatus.Unused &&
                a.ExpiredAt > now);

            if (act == null)
                return AuthResult.Fail("Token không hợp lệ hoặc đã hết hạn.");

            var user = _db.Users.Find(act.UserId);
            if (user == null)
                return AuthResult.Fail("Không tìm thấy người dùng.");

            user.Password = PasswordHasher.HashPassword(newPassword);
            act.IsUsed = TokenStatus.Used;
            _db.SaveChanges();

            return AuthResult.Ok();
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
