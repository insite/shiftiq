using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Directory;

public class QPersonSecretWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QPersonSecretWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(QPersonSecretEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SecretIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QPersonSecret.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid secret, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QPersonSecret.SingleOrDefaultAsync(x => x.SecretIdentifier == secret, cancellation);
        if (entity == null)
            return false;

        db.QPersonSecret.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(QPersonSecretEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SecretIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid secret, CancellationToken cancellation, TableDbContext db)
		=> await db.QPersonSecret.AsNoTracking().AnyAsync(x => x.SecretIdentifier == secret, cancellation);
}