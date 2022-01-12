using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using RestoreMonarchy.PaymentGateway.Web.Models.View;
using RestoreMonarchy.PaymentGateway.Web.Repositories;
using RestoreMonarchy.PaymentGateway.Web.Services;

namespace RestoreMonarchy.PaymentGateway.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService userService;
        private readonly UsersRepository usersRepository;

        public AccountController(UserService userService, UsersRepository usersRepository)
        {
            this.userService = userService;
            this.usersRepository = usersRepository;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel form, string returnUrl = "")
        {
            if (!ModelState.IsValid)
                return View(form);

            MUser user = await usersRepository.GetUserAsync(form.Username, form.Password);

            if (user == null)
            {
                ModelState.AddModelError("summary", "Invalid username or password");
                return View(form);
            }

            await userService.SignInAsync(HttpContext, user, form.IsPersistent);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return Redirect("/Admin");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await userService.SignOut(HttpContext);
            return Redirect("/");
        }
    }
}
