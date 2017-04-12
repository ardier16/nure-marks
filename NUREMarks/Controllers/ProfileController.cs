using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NUREMarks.Models;
using NUREMarks.Models.ManageViewModels;

namespace NUREMarks.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> UserManager;
        private readonly SignInManager<User> SignInManager;
        MarksContext db;

        public ProfileController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MarksContext context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            db = context;
        }


        [HttpGet]
        public IActionResult Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль был успешно изменён."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";

            if (SignInManager.IsSignedIn(User))
            {
                User user = UserManager.Users.ToList().Find(u => u.Id.Equals(UserManager.GetUserId(User)));
                Student student = db.Students.ToList().Find(u => u.Id.Equals(user.StudentId));
                ViewData["name"] = student.Name;
                ViewData["rating"] = db.Ratings.Where(r => r.StudentId.Equals(student.Id)).First().Value;

                List<Mark> marks = db.Marks.ToList().FindAll(m => m.StudentId.Equals(student.Id));

                return View(marks);
            }
            else
            {
                return RedirectToAction(nameof(AccountController.Login), "Account/Login");
            }
        }


        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }



        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

        private Task<User> GetCurrentUserAsync()
        {
            return UserManager.GetUserAsync(HttpContext.User);
        }

        #endregion
}
}