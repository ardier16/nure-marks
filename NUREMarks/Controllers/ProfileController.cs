using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NUREMarks.Models;

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



        public IActionResult Index()
        {
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


            return View();
        }
    }
}