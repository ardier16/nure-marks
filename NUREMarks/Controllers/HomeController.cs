using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUREMarks.Models;
using Microsoft.AspNetCore.Authorization;

namespace NUREMarks.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string name = User.Identity.Name;

            if (name != null)
            {
                return Content(name);
            }
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "NURE Marks - електронний журнал ХНУРЕ";
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
