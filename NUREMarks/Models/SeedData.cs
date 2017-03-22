using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NUREMarks.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider servicePorvider)
        {
            var context = servicePorvider.GetService<MarksContext>();

            if (!context.Groups.Any())
            {
                Group g1 = new Group
                {
                    Name = "ПИ-15-1",
                    Course = 2,
                    Department = "Программная инженерия",
                    FacultyShort = "КН",
                    FacultyFull = "Компьютерных наук"
                };
                Group g2 = new Group
                {
                    Name = "ПИ-15-2",
                    Course = 2,
                    Department = "Программная инженерия",
                    FacultyShort = "КН",
                    FacultyFull = "Компьютерных наук"
                };
                Group g5 = new Group
                {
                    Name = "ПИ-15-5",
                    Course = 2,
                    Department = "Программная инженерия",
                    FacultyShort = "КН",
                    FacultyFull = "Компьютерных наук"
                };

                context.Groups.AddRange(g1, g2, g5);
                context.SaveChanges();

                Student st1 = new Student
                {
                    EMail = "maksym.shopynskyi@nure.ua",
                    Password = "123456",
                    Name = "Максим",
                    FullName = "Шопинский Максим Владимирович",
                    Group = g5,
                    IsBudgetary = true
                };
                Student st2 = new Student
                {
                    EMail = "volodymyr.maiboroda@nure.ua",
                    Password = "123456",
                    Name = "Владимир",
                    FullName = "Майборода Владимир Андреевич",
                    Group = g5,
                    IsBudgetary = true
                };
                Student st3 = new Student
                {
                    EMail = "maksym.pihnastyi@nure.ua",
                    Password = "123456",
                    Name = "Максим",
                    FullName = "Пигнастый Максим Олегович",
                    Group = g5,
                    IsBudgetary = false
                };
                Student st4 = new Student
                {
                    EMail = "yevheniy.nadtochii@nure.ua",
                    Password = "123456",
                    Name = "Евгений",
                    FullName = "Надточий Евгений Валерьевич",
                    Group = g5,
                    IsBudgetary = true
                };
                Student st5 = new Student
                {
                    EMail = "maksym.shopynskyi@nure.ua",
                    Password = "123456",
                    Name = "Виктория",
                    FullName = "Клепикова Виктория Константиновна",
                    Group = g5,
                    IsBudgetary = false
                };
                Student st6 = new Student
                {
                    EMail = "ivan.popovich@nure.ua",
                    Password = "123456",
                    Name = "Иван",
                    FullName = "Попович Иван Дмитриевич",
                    Group = g2,
                    IsBudgetary = true
                };

                context.Students.AddRange(st1, st2, st3, st4, st5, st6);
                context.SaveChanges();

                Teacher t1 = new Teacher
                {
                    Name = "Лановой Алексей Феликсович",
                    EMail = "sample@mail.com",
                    Department = "Программная инженерия"
                };
                Teacher t2 = new Teacher
                {
                    Name = "Мазурова Оксана Алексеевна",
                    EMail = "sample@mail.com",
                    Department = "Программная инженерия"
                };
                Teacher t3 = new Teacher
                {
                    Name = "Мельникова Роксана Валерьевна",
                    EMail = "sample@mail.com",
                    Department = "Программная инженерия"
                };

                context.Teachers.AddRange(t1, t2, t3);
                context.SaveChanges();

                Semester sem1 = new Semester
                {
                    Month = "Январь",
                    Year = 2017
                };
                Semester sem2 = new Semester
                {
                    Month = "Июнь",
                    Year = 2017
                };

                context.Semesters.AddRange(sem1, sem2);
                context.SaveChanges();

                Rating r1 = new Rating
                {
                    Student = st1,
                    Semester = sem1,
                    Value = 97.143,
                    Bonus = 0,
                    SubjectsCount = 6
                };
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
                context.SaveChanges();
            }
        }
    }
}
