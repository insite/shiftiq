using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Booking;

public class RegistrationInstructorWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public RegistrationInstructorWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(RegistrationInstructorEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.InstructorIdentifier, entity.RegistrationIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QRegistrationInstructor.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid instructor, Guid registration, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QRegistrationInstructor.SingleOrDefaultAsync(x => x.InstructorIdentifier == instructor && x.RegistrationIdentifier == registration, cancellation);
        if (entity == null)
            return false;

        db.QRegistrationInstructor.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(RegistrationInstructorEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.InstructorIdentifier, entity.RegistrationIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid instructor, Guid registration, CancellationToken cancellation, TableDbContext db)
		=> await db.QRegistrationInstructor.AsNoTracking().AnyAsync(x => x.InstructorIdentifier == instructor && x.RegistrationIdentifier == registration, cancellation);
}