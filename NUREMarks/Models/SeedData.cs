using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
                                Password = GetEncryptedData("123456"),
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

        public static string GetEncryptedData(string password)
        {
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public static bool SetNewPassword(string email, string newPassword,  MarksContext context)
        {
            Student st = context.Students.SingleOrDefault(c => c.EMail.Equals(email));
            if (st == null)
                return false;
            st.Password = GetEncryptedData(newPassword);
            context.SaveChanges();
            return true;
           
            
        }

        public static void Initialize(IServiceProvider servicePorvider)
        {
            var context = servicePorvider.GetService<MarksContext>();

            FillDbFromPDF(context);

                  //  SearchByName("майборода", context);
            //SetNewPassword("volodymyr.maiboroda@nure.ua", "123456", context);
     
        }
    }
}
