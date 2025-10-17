using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Security;

public class TPermissionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public TPermissionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(TPermissionEntity entity, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PermissionIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.TPermission.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(TPermissionEntity entity, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.PermissionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid permission, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TPermission.SingleOrDefaultAsync(x => x.PermissionIdentifier == permission, cancellation);
        if (entity == null)
            return false;

        db.TPermission.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    private async Task<bool> AssertAsync(Guid permission, CancellationToken cancellation, TableDbContext db)
		=> await db.TPermission.AsNoTracking().AnyAsync(x => x.PermissionIdentifier == permission, cancellation);
}