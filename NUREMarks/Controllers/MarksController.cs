﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUREMarks.Models;
using NUREMarks.Models.MarksViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NUREMarks.Controllers
{
    public class MarksController : Controller
    {
        MarksContext db;

        public MarksController(MarksContext context)
        {
            db = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddViewModel model)
        {
            string SubAbbr = model.Subject.Split('(', ')')[0].TrimEnd(' ');
            string TeacherName = model.Subject.Split('(', ')')[1];

            Subject sub = db.Subjects.Where(s => s.Abbreviation.Equals(SubAbbr) && s.Teacher.Equals(TeacherName)).First();
            Semester sem = db.Semesters.ToList()[1];

            db.Marks.Add(new Mark
            {
                Semester = sem,
                Subject = sub,
                StudentId = model.StudentId,
                Value = model.Value
            });

            db.SaveChanges();

            return RedirectToAction("Student", "Profile", new
            {
                id = model.StudentId,
                message = "Оценка успешно добавлена"
            });
        }

        [HttpGet]
        public IActionResult Add(int studentId)
        {
            ViewBag.StudentId = studentId;
            ViewBag.SubList = db.Subjects.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Edit(EditViewModel model)
        {
            Mark mark = db.Marks.Where(m => m.Id.Equals(model.MarkId)).First();
            mark.Value = model.Value;
            
            db.SaveChanges();

            return RedirectToAction("Student", "Profile", new
            {
                id = mark.StudentId,
                message = "Оценка успешно изменена"
            });
        }

        [HttpGet]
        public IActionResult Edit(int MarkId)
        {
            ViewBag.MarkId = MarkId;
            Mark mark = db.Marks.Where(m => m.Id.Equals(MarkId)).First();
            ViewBag.Subject = db.Subjects.Where(s => s.Id.Equals(mark.SubjectId)).First().Name;
            ViewBag.MarkValue = mark.Value;

            return View();
        }

        [HttpGet]
        public IActionResult Delete(int MarkId)
        {
            Mark mark = db.Marks.Where(m => m.Id.Equals(MarkId)).First();
            db.Marks.Remove(mark);
            db.SaveChanges();

            return RedirectToAction("Student", "Profile", new
            {
                id = mark.StudentId,
                message = "Оценка успешно удалена"
            });
        }

    }
}
