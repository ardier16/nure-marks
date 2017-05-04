using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUREMarks.Models
{
    public class RatingInfo
    {
        public int StudentId { get; set; }
        public string Semester { get; set; }
        public double? Delta { get; set; }
        public double RatingValue { get; set; }

    }
}
