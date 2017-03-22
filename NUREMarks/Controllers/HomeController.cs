using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUREMarks.Models;

namespace NUREMarks.Controllers
{
    public class HomeController : Controller
    {
        MarksContext db;
        public HomeController(MarksContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View(db.Ratings.Join(db.Students, r => r.StudentId,
                s => s.Id, (r, s) => new
                {
                    Name = s.FullName,
                    Group = db.Groups.Where(i => i.Id == s.GroupId).Select(i => i.Name),
                    Rating = r.Value,
                    Note = r.Note
                }).ToList());

        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
