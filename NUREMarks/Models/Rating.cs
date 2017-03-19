namespace NUREMarks.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int SubjectsCount { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
    }
}
