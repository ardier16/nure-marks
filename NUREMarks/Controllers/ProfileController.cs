using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NUREMarks.Models;
using NUREMarks.Models.ManageViewModels;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Text;

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
                if (User.IsInRole("admin"))
                {
                    return RedirectToAction(nameof(HomeController.About), "Home/About");
                }


                User user = UserManager.Users.ToList().Find(u => u.Id.Equals(UserManager.GetUserId(User)));
                Student student = db.Students.ToList().Find(u => u.Id.Equals(user.StudentId));
                ViewData["id"] = student.Id;
                ViewData["name"] = student.Name;
                ViewData["rating"] = db.Ratings.Where(r => r.StudentId.Equals(student.Id)).First().Value;


                List<RatingInfo> info = (from r in db.Ratings
                                         join sem in db.Semesters on r.SemesterId equals sem.Id
                                         where r.StudentId.Equals(student.Id)
                                         orderby sem.Id descending
                                         select new RatingInfo
                                         {
                                             StudentId = r.StudentId,
                                             Semester = sem.Season + " " + sem.Year,
                                             RatingValue = r.Value
                                         }).ToList();

                SetRatingDeltas(info);

                ViewBag.RatingsHistory = info;



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
                                MarkId = m.Id,
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

        [HttpGet]
        public IActionResult Student(int id, string message = null)
        {
            if (SignInManager.IsSignedIn(User))
            {
                if (!User.IsInRole("admin"))
                {
                    return RedirectToAction(nameof(HomeController.About), "Home/About");
                }

                Student student = db.Students.Where(s => s.Id.Equals(id)).First();

                ViewData["id"] = student.Id;
                ViewData["name"] = student.Name;
                ViewData["rating"] = db.Ratings.Where(r => r.StudentId.Equals(student.Id)).First().Value;

                List<RatingInfo> info = (from r in db.Ratings
                                         join sem in db.Semesters on r.SemesterId equals sem.Id
                                         where r.StudentId.Equals(student.Id)
                                         orderby sem.Id descending
                                         select new RatingInfo
                                         {
                                             StudentId = r.StudentId,
                                             Semester = sem.Season + " " + sem.Year,
                                             RatingValue = r.Value
                                         }).ToList();

                SetRatingDeltas(info);

                ViewBag.RatingsHistory = info;



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

                if (message != null)
                {
                    ViewData["MarksStatus"] = message;
                }

                return View((from m in db.Marks
                             join st in db.Students on m.StudentId equals st.Id
                             join sub in db.Subjects on m.SubjectId equals sub.Id
                             join sem in db.Semesters on m.SemesterId equals sem.Id
                             where st.Id.Equals(student.Id)
                             orderby sem.Year, sem.Season, sub.Name
                             select new MarkInfo
                             {
                                 MarkId = m.Id,
                                 StudentName = student.Name,
                                 SubjectName = sub.Name,
                                 SubjectAbbreviation = sub.Abbreviation,
                                 Semester = sem.Season + " " + sem.Year,
                                 TeacherName = sub.Teacher,
                                 MarkValue = m.Value
                             }
                        ).ToList());
            }

            return View();
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


        private void SetRatingDeltas(List<RatingInfo> info)
        {
            for (int i = 0; i < info.Count - 1; i++)
            {
                info[i].Delta = Math.Round(info[i].RatingValue - info[i + 1].RatingValue, 3);
            }
        }


        [HttpGet]
        public IActionResult TimeTable(string group)
        {
            string url = "http://cist.nure.ua/ias/app/tt/P_API_EVENT_JSON?timetable_id=5259356&time_from=1493590100&time_to=1494160100";

            ViewBag.Text = GetHtml(url);

            return View();
        }

        private string GetHtml(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        return content.ReadAsStringAsync().Result;
                    }
                }
            }
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