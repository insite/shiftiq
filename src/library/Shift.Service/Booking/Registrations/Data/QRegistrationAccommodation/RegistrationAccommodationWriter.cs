using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Booking;

public class RegistrationAccommodationWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public RegistrationAccommodationWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(RegistrationAccommodationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AccommodationIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QRegistrationAccommodation.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid accommodation, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QRegistrationAccommodation.SingleOrDefaultAsync(x => x.AccommodationIdentifier == accommodation, cancellation);
        if (entity == null)
            return false;

        db.QRegistrationAccommodation.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(RegistrationAccommodationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AccommodationIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid accommodation, CancellationToken cancellation, TableDbContext db)
		=> await db.QRegistrationAccommodation.AsNoTracking().AnyAsync(x => x.AccommodationIdentifier == accommodation, cancellation);
}