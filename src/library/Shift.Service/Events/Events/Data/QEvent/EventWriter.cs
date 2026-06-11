using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Booking;

public class EventWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public EventWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(EventEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.EventIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QEvent.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(EventEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.EventIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid @event, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QEvent.SingleOrDefaultAsync(x => x.EventIdentifier == @event, cancellation);
        if (entity == null)
            return false;

        db.QEvent.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid @event, CancellationToken cancellation, TableDbContext db)
        => await db.QEvent.AsNoTracking().AnyAsync(x => x.EventIdentifier == @event, cancellation);
}