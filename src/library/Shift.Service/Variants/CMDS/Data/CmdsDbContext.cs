using Microsoft.EntityFrameworkCore;

namespace Shift.Service.Variant.CMDS;

public class CmdsDbContext : DbContext, IDbContext
{
    public CmdsDbContext(DbContextOptions<CmdsDbContext> options) : base(options) { }

    internal DbSet<ComplianceSummaryEntity> ComplianceSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ComplianceSummaryConfiguration());
    }
}
