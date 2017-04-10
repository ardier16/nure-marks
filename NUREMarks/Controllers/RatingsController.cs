using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUREMarks.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NUREMarks.Controllers
{
    public class RatingsController : Controller
    {
        MarksContext db;
        public RatingsController(MarksContext context)
        {
            db = context;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Ratings(string dep, int course)
        {
            //return View(new PDFParser(path).Students);

            Group group = (from g in db.Groups
                           where g.DepShort == dep && g.Course == course
                           select g).ToList().First();

            ViewData["Department"] = group.Department;
            ViewData["Faculty"] = group.FacultyFull;
            ViewData["Course"] = group.Course;
            ViewData["Type"] = group.DepShort.Last() == 'у' ? 
                                "з прискореним терміном навчання" : "";

            return View(from r in db.Ratings
                        join st in db.Students on r.StudentId equals st.Id
                        join g in db.Groups on st.GroupId equals g.Id
                        where g.DepShort.Equals(dep) && g.Course.Equals(course)
                        orderby r.Value descending, r.Note descending, g.Name, st.Name
                        select new StudentData
                        {
                            Name = st.Name,
                            Group = g.Name,
                            Rating = r.Value,
                            Info = r.Note
                        }
                        );
        }

        [HttpGet]
        public IActionResult Top100()
        {
            //return View(new PDFParser(path).Students);
            return View((from r in db.Ratings
                        join st in db.Students on r.StudentId equals st.Id
                        join g in db.Groups on st.GroupId equals g.Id
                        orderby r.Value descending, r.Note descending, g.Name, st.Name
                        select new StudentData
                        {
                            Name = st.Name,
                            Group = g.Name,
                            Rating = r.Value,
                            Info = r.Note
                        }
                        ).Take(100));
        }

        [HttpGet]
        public IActionResult FacultiesList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search(string name)
        {
            ViewData["SearchName"] = name;

            return View(from students in db.Students
                        join ratings in db.Ratings on students.Id equals ratings.StudentId
                        join groups in db.Groups on students.GroupId equals groups.Id
                        where students.Name.ToLower().Contains(name.ToLower())
                        orderby ratings.Value descending, ratings.Note descending, groups.Name, students.Name
                        select new StudentData
                        {
                            Name = students.Name,
                            Group = groups.Name,
                            Rating = ratings.Value,
                            Info = ratings.Note
                        });
        }
    }
}
