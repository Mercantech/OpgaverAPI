using Microsoft.EntityFrameworkCore;

namespace OpgaverAPI.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }
    }
}
