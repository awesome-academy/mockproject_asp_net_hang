using hotel_management.Constants;
using hotel_management.Enums;
using hotel_management.Models;
using hotel_management.Models.AuthView;
using hotel_management.Services.Mail;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<bool> IsEmailValidAsync(string email)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"https://api.mailboxlayer.com/check?access_key=YOUR_KEY&email={email}");
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                return result.format_valid && result.mx_found && result.smtp_check;
            }
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<AuthResult> RegisterUser(RegisterViewModel model, string baseUrl)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (await _db.Users.AnyAsync(u => u.Username == model.Username && u.IsActive))
                    {
                        return AuthResult.Fail("Username đã tồn tại");
                    }

                    if (await _db.Users.AnyAsync(u => u.Email == model.Email && u.IsActive))
                    {
                        return AuthResult.Fail("Email đã tồn tại");
                    }
                    // Hash password
                    string passwordHash = PasswordHasher.HashPassword(model.Password);

                    var user = new User
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = passwordHash,
                        Phone = model.Phone,
                        Address = model.Address,
                        RoleId = (int)RoleType.Customer,
                        IsActive = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();

                    var token = GenerateToken();

                    var activation = new UserActivation
                    {
                        UserId = user.Id,
                        Token = token,
                        TokenType = (int)TokenType.EmailVerification,
                        ExpiredAt = DateTime.Now.AddHours(Common.Expired_Token_Ative_Accoutn),
                        IsUsed = (int)TokenStatus.UnUsed,
                        CreatedAt = DateTime.Now
                    };

                    _db.UserActivations.Add(activation);
                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    await SendActivationEmail(user.Email, token, baseUrl);

                    return AuthResult.Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return AuthResult.Fail("Có lỗi khi đăng ký: " + ex.Message);
                }
            }
        }


        private async Task SendActivationEmail(string email, string token, string baseUrl)
        {
            string activationLink = $"{baseUrl}/Auth/Activate?token={token}";

            var subject = "Kích hoạt tài khoản";
            var body = $"Vui lòng click link sau để kích hoạt tài khoản: {activationLink}";

            await _mail.SendMailAsync(email, subject, body);
        }

        public AuthResult ActivateUser(string token)
        {
            try {
                var activation = _db.UserActivations.FirstOrDefault(x => x.Token == token && x.TokenType == (int)TokenType.EmailVerification && x.IsUsed == (int)TokenStatus.UnUsed);
                if (activation == null)
                    return AuthResult.Fail("Token không hợp lệ");

                if (activation.IsUsed == (int)TokenStatus.Used)
                    return AuthResult.Fail("Token đã được sử dụng");

                if (activation.ExpiredAt < DateTime.Now)
                {
                    return AuthResult.Fail("Expired");
                }

                activation.IsUsed = (int) TokenStatus.Used;
                activation.User.IsActive = true;
                activation.User.UpdatedAt = DateTime.Now;
                _db.SaveChanges();

                return AuthResult.Ok();
            }
            catch (Exception ex)
            {
                return AuthResult.Fail("Có lỗi khi kích hoạt tài khoản: " + ex.Message);
            }
        }

        public async Task<AuthResult> ResendActivationEmail(string email, string baseUrl)
        {
            try
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsActive == false);

                if (user == null)
                    return AuthResult.Fail("Không tìm thấy tài khoản chưa kích hoạt với email này.");

                var oldTokens = _db.UserActivations
                    .Where(t => t.UserId == user.Id &&
                                t.TokenType == (int)TokenType.EmailVerification &&
                                t.IsUsed == (int)TokenStatus.UnUsed);

                foreach (var t in oldTokens)
                {
                    t.IsUsed = (int)TokenStatus.Used;
                }

                var token = GenerateToken();
                var activation = new UserActivation
                {
                    UserId = user.Id,
                    Token = token,
                    TokenType = (int)TokenType.EmailVerification,
                    ExpiredAt = DateTime.Now.AddHours(Common.Expired_Token_Ative_Accoutn),
                    IsUsed = (int)TokenStatus.UnUsed,
                    CreatedAt = DateTime.Now
                };

                _db.UserActivations.Add(activation);
                await _db.SaveChangesAsync();

                await SendActivationEmail(user.Email, token, baseUrl);

                return AuthResult.Ok();
            }
            catch (Exception ex)
            {
                return AuthResult.Fail("Có lỗi khi gửi lại email kích hoạt: " + ex.Message);
            }
        }

        public User GetUserByToken(string token)
        {
            var activation = _db.UserActivations.FirstOrDefault(x => x.Token == token);
            if (activation == null)
                return null;
            return activation.User;
        }
    }
}
