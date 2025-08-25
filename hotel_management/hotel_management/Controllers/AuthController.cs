using hotel_management.Models;
using hotel_management.Services.Auth;
using hotel_management.Services.Mail;
using hotel_management.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace hotel_management.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db, IMailService mailService)
        {
            _db = db;
            _auth = new AuthService(db, mailService);
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string emailOrUsername, string password)
        {
            var result = _auth.Login(emailOrUsername, password);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            _auth.Logout();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";
            var result = await _auth.SendPasswordResetAsync(email, baseUrl);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }

            TempData["Message"] = "Email đặt lại mật khẩu đã được gửi, vui lòng kiểm tra hộp thư đến của bạn";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Message"] = "Token không hợp lệ.";
                return RedirectToAction("Login");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError("", "Mật khẩu không hợp lệ.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            var result = _auth.ResetPassword(model.Token, model.NewPassword);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);
                return View(model);
            }

            TempData["Message"] = "Đặt lại mật khẩu thành công, vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
    }
}
