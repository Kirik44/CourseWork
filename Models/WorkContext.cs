using Microsoft.EntityFrameworkCore;

namespace CourseWork.Models
{
    public class WorkContext : DbContext
    {
        public DbSet<Moder> Moders { get; set; }
        public DbSet<Request> Requests { get; set; }
        public WorkContext(DbContextOptions<WorkContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
