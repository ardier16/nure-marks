﻿namespace NUREMarks.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsBudgetary { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
