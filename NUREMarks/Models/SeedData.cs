using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;

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
                                EMail = EmailGenerator.GenerateNureEmail(list[j].Name),
                                Password = "123456",
                                Name = list[j].Name,
                                Group = context.Groups.First(p => p.Name == list[j].Group),
                                IsBudgetary = list[j].Info == "контракт" ? false : true
                            };

                            context.Students.Add(s);
                            context.SaveChanges();

                            Rating r = new Rating
                            {
                                Student = s,
                                Semester = sem1,
                                Value = list[j].Rating,
                                Bonus = 0,
                                Note = list[j].Info
                            };

                            context.Ratings.Add(r);

                        }

                        context.SaveChanges();
                    }
                }
            }
        }


        public static void Initialize(IServiceProvider servicePorvider)
        {
            var context = servicePorvider.GetService<MarksContext>();

            FillDbFromPDF(context);


            /*
            

               

                
                Rating r2 = new Rating
                {
                    Student = st2,
                    Semester = sem1,
                    Value = 88.714,
                    Bonus = 0,
                    SubjectsCount = 6
                };
                Rating r3 = new Rating
                {
                    Student = st3,
                    Semester = sem1,
                    Value = 0,
                    Bonus = 0,
                    SubjectsCount = 6
                };
                Rating r4 = new Rating
                {
                    Student = st4,
                    Semester = sem1,
                    Value = 84.714,
                    Bonus = 0,
                    SubjectsCount = 6
                };
                Rating r5 = new Rating
                {
                    Student = st5,
                    Semester = sem1,
                    Value = 0,
                    Bonus = 0,
                    SubjectsCount = 6
                };
                Rating r6 = new Rating
                {
                    Student = st6,
                    Semester = sem1,
                    Value = 91.857,
                    Bonus = 0,
                    SubjectsCount = 6
                };

                context.Ratings.AddRange(r1, r2, r3, r4, r5, r6);
             */
        }
    }
}
