using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace NUREMarks.Models
{
    public class MarksContext : IdentityDbContext<User>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Mark> Marks { get; set; }

        public MarksContext(DbContextOptions<MarksContext> options)
            : base(options)
        {
        }

        public MarksContext()
        {

        }
    }
}
