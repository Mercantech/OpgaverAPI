using Microsoft.EntityFrameworkCore;
using OpgaverAPI.Models;

namespace OpgaverAPI.Context
{
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }
    }
}
