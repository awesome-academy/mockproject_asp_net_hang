using hotel_management.Models;
using hotel_management.Models.AuthView;
using hotel_management.Services.Auth;
using hotel_management.Services.Mail;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace hotel_management.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db, IAuthService authService)
        {
            _db = db;
            _auth = authService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var validEmail = await _auth.IsEmailValidAsync(model.Email);
                //if (!validEmail)
                //{
                //    TempData["ErrorMessage"] ="Email không tồn tại";
                //    return View(model);
                //}

                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                var result = await _auth.RegisterUser(model, baseUrl);

                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Error;
                    return View(model);
                }

                return RedirectToAction("Login");
            }

            return View(model);
        }

        public ActionResult Activate(string token)
        {
            var result = _auth.ActivateUser(token);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Kích hoạt thành công. Bạn có thể đăng nhập.";
                return RedirectToAction("Register");
            }

            if (!string.IsNullOrEmpty(result.Error) && result.Error == "Expired")
            {
                var user = _auth.GetUserByToken(token);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Token không hợp lệ vui lòng kiểm tra lại link kích hoạt trong email.";
                    return View("ActivateExpired");
                }

                ViewBag.Email = user.Email;
                return View("ActivateExpired");
            }

            TempData["ErrorMessage"] = result.Error;
            return View("ActivateExpired");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendActivationEmail(string email)
        {
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var result = await _auth.ResendActivationEmail(email, baseUrl);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Email kích hoạt đã được gửi lại. Vui lòng kiểm tra hộp thư.";
                return View("Register");
            }

            TempData["ErrorMessage"] = result.Error;
            return View("ActivateExpired");
        }
    }
}
