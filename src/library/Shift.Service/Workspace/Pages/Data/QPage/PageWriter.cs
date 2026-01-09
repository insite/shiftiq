using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workspace;

public class PageWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public PageWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(PageEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PageIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QPage.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid page, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QPage.SingleOrDefaultAsync(x => x.PageIdentifier == page, cancellation);
        if (entity == null)
            return false;

        db.QPage.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(PageEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PageIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid page, CancellationToken cancellation, TableDbContext db)
        => await db.QPage.AsNoTracking().AnyAsync(x => x.PageIdentifier == page, cancellation);
}