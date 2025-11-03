using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Booking;

public class EventUserWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public EventUserWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(EventUserEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.EventIdentifier, entity.UserIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QEventUser.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid @event, Guid user, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QEventUser.SingleOrDefaultAsync(x => x.EventIdentifier == @event && x.UserIdentifier == user, cancellation);
        if (entity == null)
            return false;

        db.QEventUser.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(EventUserEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.EventIdentifier, entity.UserIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid @event, Guid user, CancellationToken cancellation, TableDbContext db)
        => await db.QEventUser.AsNoTracking().AnyAsync(x => x.EventIdentifier == @event && x.UserIdentifier == user, cancellation);
}