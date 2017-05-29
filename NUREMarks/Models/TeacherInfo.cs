using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUREMarks.Models
{
    public class TeacherInfo
    {
        public string Teacher { get; set; }
        public string Subject { get; set; }
        public List<string> Groups { get; set; }
        public string Semester { get; set; }
    }
}
