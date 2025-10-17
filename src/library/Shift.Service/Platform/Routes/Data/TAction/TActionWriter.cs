using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Metadata;

public class TActionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public TActionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(TActionEntity entity, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ActionIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.TAction.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(TActionEntity entity, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ActionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid action, CancellationToken cancellation)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TAction.SingleOrDefaultAsync(x => x.ActionIdentifier == action, cancellation);
        if (entity == null)
            return false;

        db.TAction.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    private async Task<bool> AssertAsync(Guid action, CancellationToken cancellation, TableDbContext db)
		=> await db.TAction.AsNoTracking().AnyAsync(x => x.ActionIdentifier == action, cancellation);
}