using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Booking;

public class RegistrationTimerWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public RegistrationTimerWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(RegistrationTimerEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.TriggerCommand, cancellation, db);
        if (exists)
            return false;
                
        await db.QRegistrationTimer.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid triggerCommand, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QRegistrationTimer.SingleOrDefaultAsync(x => x.TriggerCommand == triggerCommand, cancellation);
        if (entity == null)
            return false;

        db.QRegistrationTimer.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(RegistrationTimerEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.TriggerCommand, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid triggerCommand, CancellationToken cancellation, TableDbContext db)
		=> await db.QRegistrationTimer.AsNoTracking().AnyAsync(x => x.TriggerCommand == triggerCommand, cancellation);
}