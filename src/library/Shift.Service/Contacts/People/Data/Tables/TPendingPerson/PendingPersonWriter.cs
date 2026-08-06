using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Directory;

public class PendingPersonWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public PendingPersonWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(PendingPersonEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        await db.PendingPerson.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid pending, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.PendingPerson.SingleOrDefaultAsync(x => x.PendingPersonIdentifier == pending, cancellation);
        if (entity == null)
            return false;

        db.PendingPerson.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(PendingPersonEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PendingPersonIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid pending, CancellationToken cancellation, TableDbContext db)
		=> await db.PendingPerson.AsNoTracking().AnyAsync(x => x.PendingPersonIdentifier == pending, cancellation);
}
