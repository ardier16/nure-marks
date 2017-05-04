using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUREMarks.Models
{
    public class MarkInfo
    {
        public int MarkId { get; set; }
        public string StudentName { get; set; }
        public string Semester { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public string SubjectAbbreviation { get; set; }
        public int MarkValue { get; set; }

    }
}
