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

                ViewData["uni-stat"] = (db.Ratings.ToList().OrderByDescending(r => r.Value).Select(r => r.StudentId).ToList().IndexOf(student.Id) + 1)
                    + " из " + db.Ratings.ToList().Count;

                var gr = db.Groups.Where(g => g.Id.Equals(student.GroupId)).First();
                var grs = db.Groups.Where(g => (g.Course.Equals(gr.Course) && g.Department.Equals(gr.Department))).Select(g => g.Id);
                var studs = db.Students.Where(s => grs.Contains(s.GroupId)).Select(s => s.Id).ToList();
                var rs = db.Ratings.Where(r => studs.Contains(r.StudentId)).ToList();

                ViewData["dep-stat"] = (rs.OrderByDescending(r => r.Value).Select(r => r.StudentId).ToList().IndexOf(student.Id) + 1)
                    + " из " + rs.ToList().Count;
                ViewData["group"] = gr.Name;
                ViewData["course"] = gr.Course;
                ViewData["email"] = user.Email;

                return View((from m in db.Marks
                            join st in db.Students on m.StudentId equals st.Id
                            join sub in db.Subjects on m.SubjectId equals sub.Id
                            join sem in db.Semesters on m.SemesterId equals sem.Id
                            where st.Id.Equals(student.Id)
                            orderby sem.Year, sem.Season, sub.Name
                            select new MarkInfo
                            {
                                StudentName = student.Name,
                                SubjectName = sub.Name,
                                SubjectAbbreviation = sub.Abbreviation,
                                Semester = sem.Season + " " + sem.Year,
                                TeacherName = sub.Teacher,
                                MarkValue = m.Value
                            }
                        ).ToList());
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