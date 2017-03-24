namespace NUREMarks.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string Note { get; set; }
        public double Bonus { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
    }
}
