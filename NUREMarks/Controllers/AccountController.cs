using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NUREMarks.ViewModels;
using NUREMarks.Models;

namespace NUREMarks.Controllers
{
    public class AccountController : Controller
    {
        private MarksContext db;

        public AccountController(MarksContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string password = SeedData.GetEncryptedData(model.Password);
                Student student = await db.Students.FirstOrDefaultAsync(
                    s => s.EMail == model.EMail && s.Password == password);

                if (student != null)
                {
                    await Authenticate(model.EMail);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Неправильний логін та/або пароль");
            }

            return View();
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
                                        ClaimsIdentity.DefaultNameClaimType, 
                                        ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction("Login", "Account");
        }
    }
}