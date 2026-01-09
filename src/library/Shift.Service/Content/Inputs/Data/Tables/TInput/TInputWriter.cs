using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Content;

public class TInputWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public TInputWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(TInputEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ContentIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.TInput.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(TInputEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ContentIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid content, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TInput.SingleOrDefaultAsync(x => x.ContentIdentifier == content, cancellation);
        if (entity == null)
            return false;

        db.TInput.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    private async Task<bool> AssertAsync(Guid content, CancellationToken cancellation, TableDbContext db)
		=> await db.TInput.AsNoTracking().AnyAsync(x => x.ContentIdentifier == content, cancellation);
}