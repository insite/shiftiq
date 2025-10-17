using InSite.Application.Files.Read;

using Microsoft.EntityFrameworkCore;

using Shift.Service.Content;

namespace Shift.Service;

public class ViewDbContext : DbContext, IDbContext
{
    public ViewDbContext(DbContextOptions<ViewDbContext> options) : base(options) { }

    #region Views

    // Feature: Content
    internal DbSet<OrphanFile> VOrphanFile { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ApplyConfigurations(builder);
    }

    private void ApplyConfigurations(ModelBuilder builder)
    {
        // Feature: Content
        builder.ApplyConfiguration(new OrphanFileConfiguration());
    }
}