using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Progress;

public class PeriodWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public PeriodWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(PeriodEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PeriodIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QPeriod.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid period, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QPeriod.SingleOrDefaultAsync(x => x.PeriodIdentifier == period, cancellation);
        if (entity == null)
            return false;

        db.QPeriod.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(PeriodEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PeriodIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid period, CancellationToken cancellation, TableDbContext db)
        => await db.QPeriod.AsNoTracking().AnyAsync(x => x.PeriodIdentifier == period, cancellation);
}