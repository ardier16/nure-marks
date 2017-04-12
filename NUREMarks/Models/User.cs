using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace NUREMarks.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
