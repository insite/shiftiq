using Microsoft.EntityFrameworkCore;

namespace Shift.Hub
{
    public class EngineDbContext : DbContext
    {
        public EngineDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
}
