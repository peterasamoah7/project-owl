using Microsoft.EntityFrameworkCore;

namespace ProjectOwl.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext>options): base(options)
        {
        }

        public DbSet<Audio> Audios { get; set; }
    }
}
