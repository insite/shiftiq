using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workspace;

public class SiteWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public SiteWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(SiteEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SiteIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QSite.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid site, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QSite.SingleOrDefaultAsync(x => x.SiteIdentifier == site, cancellation);
        if (entity == null)
            return false;

        db.QSite.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(SiteEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SiteIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid site, CancellationToken cancellation, TableDbContext db)
        => await db.QSite.AsNoTracking().AnyAsync(x => x.SiteIdentifier == site, cancellation);
}