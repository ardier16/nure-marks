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
            Group group = (from g in db.Groups
                           where g.DepShort == dep && g.Course == course
                           select g).ToList().First();

            ViewData["Department"] = group.Department;
            ViewData["Faculty"] = group.FacultyFull;
            ViewData["Course"] = group.Course;
            ViewData["Dep"] = group.DepShort;
            ViewData["Type"] = group.DepShort.Last() == 'у' ?
                                "з прискореним терміном навчання" : "";

            List<StudentData> data = GetRatings(dep, course);

            ViewBag.Paid = (int) Math.Round(data.Where(d => d.Info != "контракт").Count() * 0.4);


            return View(data);
        }

        private List<StudentData> GetRatings(string dep, int course)
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.DepShort.Equals(dep) && g.Course.Equals(course) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }).ToList();

            return data;
        }

        [HttpGet]
        public IActionResult Top100()
        {
            return View((from r in db.Ratings
                         join st in db.Students on r.StudentId equals st.Id
                         join g in db.Groups on st.GroupId equals g.Id
                         where r.SemesterId.Equals(1)
                         orderby r.Value descending, r.Note descending, g.Name, st.Name
                         select new StudentData
                         {
                             Id = st.Id,
                             Name = st.Name,
                             Group = g.Name,
                             Rating = r.Value,
                             Info = r.Note
                         }
                        ).Take(100));
        }

        [HttpGet]
        public IActionResult GroupStat(int id)
        {
            int groupId = db.Students.Where(s => s.Id.Equals(id)).First().GroupId;
            ViewBag.Id = id;
            ViewBag.Group = db.Groups.Where(g => g.Id.Equals(groupId)).First().Name;

            return View((from r in db.Ratings
                         join st in db.Students on r.StudentId equals st.Id
                         join g in db.Groups on st.GroupId equals g.Id
                         where g.Id.Equals(groupId) && r.SemesterId.Equals(1)
                         orderby r.Value descending, st.Name
                         select new StudentData
                         {
                             Id = st.Id,
                             Name = st.Name,
                             Group = g.Name,
                             Rating = r.Value,
                             Info = r.Note
                         }
                        ).ToList());
        }

        [HttpGet]
        public IActionResult FacultyStat(int id)
        {
            ViewBag.Id = id;
            int groupId = db.Students.Where(s => s.Id.Equals(id)).First().GroupId;
            string facShort = db.Groups.Where(gr => gr.Id.Equals(groupId)).First().FacultyShort;
            ViewBag.Fac = facShort;

            return View((from r in db.Ratings
                         join st in db.Students on r.StudentId equals st.Id
                         join g in db.Groups on st.GroupId equals g.Id
                         where g.FacultyShort.Equals(facShort) && r.SemesterId.Equals(1)
                         orderby r.Value descending, r.Note descending, g.Name, st.Name
                         select new StudentData
                         {
                             Id = st.Id,
                             Name = st.Name,
                             Group = g.Name,
                             Rating = r.Value,
                             Info = r.Note
                         }
                        ).ToList());
        }

        [HttpGet]
        public IActionResult SpecialityStat(int id)
        {
            ViewBag.Id = id;
            int groupId = db.Students.Where(s => s.Id.Equals(id)).First().GroupId;
            string spec = db.Groups.Where(gr => gr.Id.Equals(groupId)).First().DepShort;
            ViewBag.Spec = spec;

            return View((from r in db.Ratings
                         join st in db.Students on r.StudentId equals st.Id
                         join g in db.Groups on st.GroupId equals g.Id
                         where g.DepShort.Equals(spec) && r.SemesterId.Equals(1)
                         orderby r.Value descending, r.Note descending, g.Name, st.Name
                         select new StudentData
                         {
                             Id = st.Id,
                             Name = st.Name,
                             Group = g.Name,
                             Rating = r.Value,
                             Info = r.Note
                         }
                        ).ToList());
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

            return View((from students in db.Students
                         join ratings in db.Ratings on students.Id equals ratings.StudentId
                         join groups in db.Groups on students.GroupId equals groups.Id
                         where students.Name.ToLower().Contains(name.ToLower()) && ratings.SemesterId.Equals(1)
                         orderby ratings.Value descending, ratings.Note descending, groups.Name, students.Name
                         select new StudentData
                         {
                             Id = students.Id,
                             Name = students.Name,
                             Group = groups.Name,
                             Rating = ratings.Value,
                             Info = ratings.Note
                         }).ToList());
        }

        [HttpGet]
        public IActionResult Calculate()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SaveSpecialityDoc(string dep, int course)
        {
            Group group = (from g in db.Groups
                           where g.DepShort == dep && g.Course == course
                           select g).ToList().First();

            List<StudentData> data = GetRatings(dep, course);
            string header = "ФАКУЛЬТЕТ " + group.FacultyFull.ToUpper();
            string info = "Рейтинг студентів " + course + " курсу cпеціальності \"" + group.Department + "\"";
            string name = Services.StringTranslator.ConvertString(dep) + (17 - course);
            Services.DocSaver.CreateWordDoc("wwwroot/docs/" + name + ".docx", data, header, info);
            return Redirect("/docs/" + name + ".docx");
        }

        [HttpGet]
        public IActionResult SaveSpecialityPdf(string dep, int course)
        {
            Group group = (from g in db.Groups
                           where g.DepShort == dep && g.Course == course
                           select g).ToList().First();

            List<StudentData> data = GetRatings(dep, course);
            string header = "ФАКУЛЬТЕТ " + group.FacultyFull.ToUpper();
            string info = "Рейтинг студентів " + course + " курсу cпеціальності \"" + group.Department + "\"";
            string name = Services.StringTranslator.ConvertString(dep) + (17 - course);
            Services.DocSaver.CreatePdfReport("wwwroot/docs/" + name + ".pdf", data, header, info);
            return Redirect("/docs/" + name + ".pdf");
        }

        [HttpGet]
        public IActionResult SaveSpecialityExcel(string dep, int course)
        {
            List<StudentData> data = GetRatings(dep, course);
            string header = "Потiк " + dep + "-" + (17 - course);
            string name = Services.StringTranslator.ConvertString(dep) + (17 - course);
            Services.DocSaver.CreateExcelDoc("wwwroot/docs/" + name + ".xlsx", data, header);
            return Redirect("/docs/" + name + ".xlsx");
        }

        [HttpGet]
        public IActionResult SaveGroupDoc(string group)
        {
            Group gr = db.Groups.Where(grp => grp.Name.Equals(group)).First();

            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.Id.Equals(gr.Id) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "ГРУПА " + group;
            string info = "Рейтинг студентів групи \"" + group + "\"";
            string name = Services.StringTranslator.ConvertString(group);

            Services.DocSaver.CreateWordDoc("wwwroot/docs/" + name + ".docx", data, header, info);
            return Redirect("/docs/" + name + ".docx");
        }

        [HttpGet]
        public IActionResult SaveGroupPdf(string group)
        {
            Group gr = db.Groups.Where(grp => grp.Name.Equals(group)).First();

            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.Id.Equals(gr.Id) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "ГРУПА " + group;
            string info = "Рейтинг студентів групи \"" + group + "\"";
            string name = Services.StringTranslator.ConvertString(group);

            Services.DocSaver.CreatePdfReport("wwwroot/docs/" + name + ".pdf", data, header, info);
            return Redirect("/docs/" + name + ".pdf");
        }

        [HttpGet]
        public IActionResult SaveGroupExcel(string group)
        {
            Group gr = db.Groups.Where(grp => grp.Name.Equals(group)).First();

            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.Id.Equals(gr.Id) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "ГРУПА " + group;
            string info = "Рейтинг студентів групи \"" + group + "\"";
            string name = Services.StringTranslator.ConvertString(group);

            Services.DocSaver.CreateExcelDoc("wwwroot/docs/" + name + ".xlsx", data, header);
            return Redirect("/docs/" + name + ".xlsx");
        }

        [HttpGet]
        public IActionResult SaveFacultyDoc(string fac)
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.FacultyShort.Equals(fac) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "ФАКУЛЬТЕТ " + fac;
            string info = "Рейтинг студентів факультету \"" + fac + "\"";
            string name = Services.StringTranslator.ConvertString(fac);

            Services.DocSaver.CreateWordDoc("wwwroot/docs/" + name + ".docx", data, header, info);
            return Redirect("/docs/" + name + ".docx");
        }

        [HttpGet]
        public IActionResult SaveFacultyPdf(string fac)
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.FacultyShort.Equals(fac) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                         ).ToList();
            string header = "ФАКУЛЬТЕТ " + fac;
            string info = "Рейтинг студентів факультету \"" + fac + "\"";
            string name = Services.StringTranslator.ConvertString(fac);

            Services.DocSaver.CreatePdfReport("wwwroot/docs/" + name + ".pdf", data, header, info);
            return Redirect("/docs/" + name + ".pdf");
        }

        [HttpGet]
        public IActionResult SaveFacultyExcel(string fac)
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.FacultyShort.Equals(fac) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "ФАКУЛЬТЕТ " + fac;
            string info = "Рейтинг студентів факультету \"" + fac + "\"";
            string name = Services.StringTranslator.ConvertString(fac);

            Services.DocSaver.CreateExcelDoc("wwwroot/docs/" + name + ".xlsx", data, header);
            return Redirect("/docs/" + name + ".xlsx");
        }

        [HttpGet]
        public IActionResult SaveSpecDoc(string spec)
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.DepShort.Equals(spec) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "СПЕЦIАЛЬНIСТЬ " + spec;
            string info = "Рейтинг студентів спецiальностi \"" + spec + "\"";
            string name = Services.StringTranslator.ConvertString(spec);

            Services.DocSaver.CreateWordDoc("wwwroot/docs/" + name + ".docx", data, header, info);
            return Redirect("/docs/" + name + ".docx");
        }

        [HttpGet]
        public IActionResult SaveSpecPdf(string spec)
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.DepShort.Equals(spec) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "СПЕЦIАЛЬНIСТЬ " + spec;
            string info = "Рейтинг студентів спецiальностi \"" + spec + "\"";
            string name = Services.StringTranslator.ConvertString(spec);

            Services.DocSaver.CreatePdfReport("wwwroot/docs/" + name + ".pdf", data, header, info);
            return Redirect("/docs/" + name + ".pdf");
        }

        [HttpGet]
        public IActionResult SaveSpecExcel(string spec)
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where g.DepShort.Equals(spec) && r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).ToList();
            string header = "СПЕЦIАЛЬНIСТЬ " + spec;
            string info = "Рейтинг студентів спецiальностi \"" + spec + "\"";
            string name = Services.StringTranslator.ConvertString(spec);

            Services.DocSaver.CreateExcelDoc("wwwroot/docs/" + name + ".xlsx", data, header);
            return Redirect("/docs/" + name + ".xlsx");
        }

        [HttpGet]
        public IActionResult SaveTopDoc()
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).Take(100).ToList();
            string header = "ТОП-100";
            string info = "Топ рейтингiв ХНУРЕ";
            string name = "Top100";

            Services.DocSaver.CreateWordDoc("wwwroot/docs/" + name + ".docx", data, header, info);
            return Redirect("/docs/" + name + ".docx");
        }

        [HttpGet]
        public IActionResult SaveTopPdf()
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).Take(100).ToList();
            string header = "ТОП-100";
            string info = "Топ рейтингiв ХНУРЕ";
            string name = "Top100";

            Services.DocSaver.CreatePdfReport("wwwroot/docs/" + name + ".pdf", data, header, info);
            return Redirect("/docs/" + name + ".pdf");
        }

        [HttpGet]
        public IActionResult SaveTopExcel()
        {
            List<StudentData> data = (from r in db.Ratings
                                      join st in db.Students on r.StudentId equals st.Id
                                      join g in db.Groups on st.GroupId equals g.Id
                                      where r.SemesterId.Equals(1)
                                      orderby r.Value descending, r.Note descending, g.Name, st.Name
                                      select new StudentData
                                      {
                                          Id = st.Id,
                                          Name = st.Name,
                                          Group = g.Name,
                                          Rating = r.Value,
                                          Info = r.Note
                                      }
                        ).Take(100).ToList();
            string header = "ТОП-100";
            string info = "Топ рейтингiв ХНУРЕ";
            string name = "Top100";

            Services.DocSaver.CreateExcelDoc("wwwroot/docs/" + name + ".xlsx", data, header);
            return Redirect("/docs/" + name + ".xlsx");
        }

        [HttpGet]
        public IActionResult SaveSearchDoc(string search)
        {
            List<StudentData> data = (from students in db.Students
                                      join ratings in db.Ratings on students.Id equals ratings.StudentId
                                      join groups in db.Groups on students.GroupId equals groups.Id
                                      where students.Name.ToLower().Contains(search.ToLower()) && ratings.SemesterId.Equals(1)
                                      orderby ratings.Value descending, ratings.Note descending, groups.Name, students.Name
                                      select new StudentData
                                      {
                                          Id = students.Id,
                                          Name = students.Name,
                                          Group = groups.Name,
                                          Rating = ratings.Value,
                                          Info = ratings.Note
                                      }).ToList();

            string header = "ПОШУК: " + search;
            string info = "Рейтинг студентів за запитом: \"" + search + "\"";
            string name = Services.StringTranslator.ConvertString(search);

            Services.DocSaver.CreateWordDoc("wwwroot/docs/" + name + ".docx", data, header, info);
            return Redirect("/docs/" + name + ".docx");
        }

        [HttpGet]
        public IActionResult SaveSearchPdf(string search)
        {
            List<StudentData> data = (from students in db.Students
                                      join ratings in db.Ratings on students.Id equals ratings.StudentId
                                      join groups in db.Groups on students.GroupId equals groups.Id
                                      where students.Name.ToLower().Contains(search.ToLower()) && ratings.SemesterId.Equals(1)
                                      orderby ratings.Value descending, ratings.Note descending, groups.Name, students.Name
                                      select new StudentData
                                      {
                                          Id = students.Id,
                                          Name = students.Name,
                                          Group = groups.Name,
                                          Rating = ratings.Value,
                                          Info = ratings.Note
                                      }).ToList();

            string header = "ПОШУК: " + search;
            string info = "Рейтинг студентів за запитом: \"" + search + "\"";
            string name = Services.StringTranslator.ConvertString(search);

            Services.DocSaver.CreatePdfReport("wwwroot/docs/" + name + ".pdf", data, header, info);
            return Redirect("/docs/" + name + ".pdf");
        }

        [HttpGet]
        public IActionResult SaveSearchExcel(string search)
        {
            List<StudentData> data = (from students in db.Students
                                      join ratings in db.Ratings on students.Id equals ratings.StudentId
                                      join groups in db.Groups on students.GroupId equals groups.Id
                                      where students.Name.ToLower().Contains(search.ToLower()) && ratings.SemesterId.Equals(1)
                                      orderby ratings.Value descending, ratings.Note descending, groups.Name, students.Name
                                      select new StudentData
                                      {
                                          Id = students.Id,
                                          Name = students.Name,
                                          Group = groups.Name,
                                          Rating = ratings.Value,
                                          Info = ratings.Note
                                      }).ToList();

            string header = "ПОШУК: " + search;
            string info = "Рейтинг студентів за запитом: \"" + search + "\"";
            string name = Services.StringTranslator.ConvertString(search);

            Services.DocSaver.CreateExcelDoc("wwwroot/docs/" + name + ".xlsx", data, header);
            return Redirect("/docs/" + name + ".xlsx");
        }
    }
}
