using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Booking;

public class RegistrationWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public RegistrationWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(RegistrationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.RegistrationIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QRegistration.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid registration, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QRegistration.SingleOrDefaultAsync(x => x.RegistrationIdentifier == registration, cancellation);
        if (entity == null)
            return false;

        db.QRegistration.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(RegistrationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.RegistrationIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid registration, CancellationToken cancellation, TableDbContext db)
		=> await db.QRegistration.AsNoTracking().AnyAsync(x => x.RegistrationIdentifier == registration, cancellation);
}