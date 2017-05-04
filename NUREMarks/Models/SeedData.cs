using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using NUREMarks.Services;

namespace NUREMarks.Models
{
    public static class SeedData
    {
        public static void FillDbFromPDF(MarksContext context)
        {
            if (!context.Students.Any())
            {
                Semester sem1 = new Semester
                {
                    Season = "Осінь",
                    Year = 2016
                };
                Semester sem2 = new Semester
                {
                    Season = "Весна",
                    Year = 2017
                };

                context.Semesters.AddRange(sem1, sem2);
                context.SaveChanges();

                string[] dirs = Directory.GetDirectories("wwwroot\\pdf");
                var students = new List<Student>();
                var groups = new List<Group>();
                var ratings = new List<Rating>();

                for (int k = 0; k < dirs.Length; k++)
                {
                    string[] paths = Directory.GetFiles(dirs[k]);

                    for (int i = 0; i < paths.Length; i++)
                    {
                        PDFParser parser = new PDFParser(paths[i]);

                        List<Group> listg = parser.Groups;
                        List<StudentData> list = parser.Students;

                        context.Groups.AddRange(listg);
                        context.SaveChanges();

                        for (int j = 0; j < list.Count; j++)
                        {
                            Student s = new Student
                            {
                                Name = list[j].Name,
                                Group = context.Groups.First(p => p.Name == list[j].Group),
                                IsBudgetary = list[j].Info == "контракт" ? false : true
                            };

                            students.Add(s);

                            Rating r = new Rating
                            {
                                Student = s,
                                Semester = sem1,
                                Value = list[j].Rating,
                                Bonus = 0,
                                Note = list[j].Info
                            };

                            ratings.Add(r);
                        }
                    }

                    
                }

                context.Students.AddRange(students);
                context.Ratings.AddRange(ratings);
                context.SaveChanges();
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<MarksContext>();
            //var users = new List<User>();

            FillDbFromPDF(context);

            AddSubjects(context);
            AddMarks(context);

            /*
            var list = context.Ratings.ToList();

            List<Rating> rlist = new List<Rating>();

            for (int i = 0; i < list.Count; i++)
            {
                rlist.Add(new Rating
                {
                    StudentId = list[i].StudentId,
                    SemesterId = 2,
                    Value = list[i].Value == 0 ? 0 : RandomValue((int)list[i].Value),
                    Bonus = 0,
                    Note = list[i].Note
                });

                rlist.Add(new Rating
                {
                    StudentId = list[i].StudentId,
                    SemesterId = 3,
                    Value = list[i].Value == 0 ? 0 : RandomValue((int)list[i].Value),
                    Bonus = 0,
                    Note = list[i].Note
                });

                rlist.Add(new Rating
                {
                    StudentId = list[i].StudentId,
                    SemesterId = 5,
                    Value = list[i].Value == 0 ? 0 : RandomValue((int)list[i].Value),
                    Bonus = 0,
                    Note = list[i].Note
                });
            }

            context.Ratings.AddRange(rlist);
            context.SaveChanges();
            /*
            var em = context.Users.ToList().GroupBy(u => u.Email).Where(r => r.Count() > 1).Select(s => s.Key).ToList();

            for (int i = 0; i < em.Count; i++)
            {
                var t = context.Users.ToList().Where(u => u.Email.Equals(em[i])).Select(st => st.StudentId).ToList();
                List<Student> x = context.Students.ToList().FindAll(s => t.Contains(s.Id));
               
                for (int j = 1; j < x.Count; j++)
                {
                    int t1 = (from g in context.Groups
                              where g.Id.Equals(x[j-1].GroupId)
                              select g.Course).ToList()[0];
                    int t2 = (from g in context.Groups
                              where g.Id.Equals(x[j].GroupId)
                              select g.Course).ToList()[0];

                    if (t1 > t2)
                    {
                        User x1 = context.Users.ToList().Find(r => r.StudentId.Equals(x[j].Id));
                        x1.Email = x1.Email.Substring(0, x1.Email.Length - 8) + j + "@nure.ua";

                        context.SaveChanges();
                    }
                    else
                    {
                        User x1 = context.Users.ToList().Find(r => r.StudentId.Equals(x[j-1].Id));
                        x1.Email = x1.Email.Substring(0, x1.Email.Length - 8) + j + "@nure.ua";

                        context.SaveChanges();
                    }
                }
            }


            var students = context.Students.ToList();
            
            for (int i = 0; i < students.Count; i++)
            {
                User u = new User
                {
                    Email = EmailGenerator.GenerateNureEmail(students[i].Name),
                    PasswordHash = "AQAAAAEAACcQAAAAEPp7lF9lUZKQpZHBR8KOpOr6ldmlaOtzSZp2pd6Q2M+IvU2WMwSrCLJOfU7wuHfVog==",
                    UserName = EmailGenerator.GenerateNureEmail(students[i].Name),
                    Student = students[i],
                    LockoutEnabled = true,
                    EmailConfirmed = true,
                    Name = students[i].Name.Split(' ')[1],
                    //NormalizedEmail = EmailGenerator.GenerateNureEmail(students[i].Name).ToUpper(),
                    //NormalizedUserName = EmailGenerator.GenerateNureEmail(students[i].Name).ToUpper()
                };

                users.Add(u);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            */

            /* var uss = context.Users.ToList();

            for (int i = 0; i < uss.Count; i++)
            {
                User st = context.Users.SingleOrDefault(c => c.Email.Equals(uss[i].Email));

                st.NormalizedEmail = uss[i].Email.ToUpper();
                st.UserName = uss[i].Email.Substring(0, uss[i].Email.Length - 8);
                st.NormalizedUserName = uss[i].UserName.ToUpper();
                context.SaveChanges();
            }
            */

        }

        public static double RandomValue(int r)
        {
            Random rand = new Random();
            double res = 0;

            while (res > 100 || res < 60)
            {
                res = rand.Next(r - 30, r + 30);
            }

            return res;
        }

        public static void AddSubjects(MarksContext db)
        {
            if (!db.Subjects.Any())
            {
                List<string> teachers = new List<string>();
                List<Subject> subjects = new List<Subject>();

                teachers.Add("Горячковская Анна Николаевна");
                teachers.Add("Мазурова Оксана Валерьевна");
                teachers.Add("Валенда Наталья Анатольевна");
                teachers.Add("Лановой Алексей Феликсович");
                teachers.Add("Мельникова Роксана Валерьевна");
                teachers.Add("Бабий Андрей Степанович");

                subjects.Add(new Subject
                {
                    Abbreviation = "Фил",
                    Course = 2,
                    Credits = 5,
                    Name = "Философия",
                    Teacher = teachers[0]
                });

                subjects.Add(new Subject
                {
                    Abbreviation = "БД",
                    Course = 2,
                    Credits = 5,
                    Name = "Базы данных",
                    Teacher = teachers[1]
                });

                subjects.Add(new Subject
                {
                    Abbreviation = "ОИГ",
                    Course = 2,
                    Credits = 5,
                    Name = "Основы игровой графики",
                    Teacher = teachers[2]
                });

                subjects.Add(new Subject
                {
                    Abbreviation = "АКОКС",
                    Course = 2,
                    Credits = 5,
                    Name = "Архитектура компьютера и организация компьютерных сетей",
                    Teacher = teachers[3]
                });

                subjects.Add(new Subject
                {
                    Abbreviation = "ЧМВ",
                    Course = 2,
                    Credits = 5,
                    Name = "Человеко-машинное взаимодействие",
                    Teacher = teachers[4]
                });

                subjects.Add(new Subject
                {
                    Abbreviation = "ТВМСиЭМПИ",
                    Course = 2,
                    Credits = 5,
                    Name = "Теория вероятностейб математическая статистика и эмпирические методы программной инженерии",
                    Teacher = teachers[5]
                });

                subjects.Add(new Subject
                {
                    Abbreviation = "БД (курс)",
                    Course = 2,
                    Credits = 5,
                    Name = "Базы данных (курсовой проект)",
                    Teacher = teachers[1]
                });

                db.Subjects.AddRange(subjects);
                db.SaveChanges();
            }
        }

        public static void AddMarks(MarksContext db)
        {
            if (!db.Marks.Any())
            {
                Student st = db.Students.ToList().Find(x => x.Name.Contains("Шопинський"));
                Student st2 = db.Students.ToList().Find(x => x.Name.Contains("Майборода Володимир"));
                Student st3 = db.Students.ToList().Find(x => x.Name.Contains("Надточій Євген"));
                Semester sem = db.Semesters.First();
                List<Subject> subj = db.Subjects.ToList();
                List<Mark> marks = new List<Mark>();

                marks.Add(new Mark
                {
                    Value = 96,
                    Student = st,
                    Semester = sem,
                    Subject = subj[0]
                });

                marks.Add(new Mark
                {
                    Value = 96,
                    Student = st,
                    Semester = sem,
                    Subject = subj[1]
                });

                marks.Add(new Mark
                {
                    Value = 96,
                    Student = st,
                    Semester = sem,
                    Subject = subj[2]
                });

                marks.Add(new Mark
                {
                    Value = 98,
                    Student = st,
                    Semester = sem,
                    Subject = subj[3]
                });

                marks.Add(new Mark
                {
                    Value = 96,
                    Student = st,
                    Semester = sem,
                    Subject = subj[4]
                });

                marks.Add(new Mark
                {
                    Value = 98,
                    Student = st,
                    Semester = sem,
                    Subject = subj[5]
                });

                marks.Add(new Mark
                {
                    Value = 100,
                    Student = st,
                    Semester = sem,
                    Subject = subj[6]
                });

                //

                marks.Add(new Mark
                {
                    Value = 88,
                    Student = st2,
                    Semester = sem,
                    Subject = subj[0]
                });

                marks.Add(new Mark
                {
                    Value = 92,
                    Student = st2,
                    Semester = sem,
                    Subject = subj[1]
                });

                marks.Add(new Mark
                {
                    Value = 90,
                    Student = st2,
                    Semester = sem,
                    Subject = subj[2]
                });

                marks.Add(new Mark
                {
                    Value = 90,
                    Student = st2,
                    Semester = sem,
                    Subject = subj[3]
                });

                marks.Add(new Mark
                {
                    Value = 75,
                    Student = st2,
                    Semester = sem,
                    Subject = subj[4]
                });

                marks.Add(new Mark
                {
                    Value = 95,
                    Student = st2,
                    Semester = sem,
                    Subject = subj[5]
                });

                marks.Add(new Mark
                {
                    Value = 96,
                    Student = st2,
                    Semester = sem,
                    Subject = subj[6]
                });

                //

                marks.Add(new Mark
                {
                    Value = 72,
                    Student = st3,
                    Semester = sem,
                    Subject = subj[0]
                });

                marks.Add(new Mark
                {
                    Value = 90,
                    Student = st3,
                    Semester = sem,
                    Subject = subj[1]
                });


                marks.Add(new Mark
                {
                    Value = 92,
                    Student = st3,
                    Semester = sem,
                    Subject = subj[3]
                });

                marks.Add(new Mark
                {
                    Value = 65,
                    Student = st3,
                    Semester = sem,
                    Subject = subj[4]
                });

                marks.Add(new Mark
                {
                    Value = 84,
                    Student = st3,
                    Semester = sem,
                    Subject = subj[5]
                });

                marks.Add(new Mark
                {
                    Value = 88,
                    Student = st3,
                    Semester = sem,
                    Subject = subj[6]
                });



                db.Marks.AddRange(marks);
                db.SaveChanges();
            }
        }



    }
    
}
