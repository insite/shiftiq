using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Competency;

public class StandardWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public StandardWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(StandardEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.StandardIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QStandard.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid standard, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QStandard.SingleOrDefaultAsync(x => x.StandardIdentifier == standard, cancellation);
        if (entity == null)
            return false;

        db.QStandard.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(StandardEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.StandardIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid standard, CancellationToken cancellation, TableDbContext db)
        => await db.QStandard.AsNoTracking().AnyAsync(x => x.StandardIdentifier == standard, cancellation);
}