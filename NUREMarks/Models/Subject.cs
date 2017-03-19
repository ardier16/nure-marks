namespace NUREMarks.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Course { get; set; }
        public string Abbreviation { get; set; }
        public int Credits { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
