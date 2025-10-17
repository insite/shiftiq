using Microsoft.EntityFrameworkCore;

namespace Engine.Api.Orchestration
{
    public class EngineDbContext : DbContext
    {
        public EngineDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
    }
}