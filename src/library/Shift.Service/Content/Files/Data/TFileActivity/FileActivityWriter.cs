using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Content;

public class FileActivityWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FileActivityWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FileActivityEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ActivityIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.TFileActivity.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid activity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TFileActivity.SingleOrDefaultAsync(x => x.ActivityIdentifier == activity, cancellation);
        if (entity == null)
            return false;

        db.TFileActivity.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(FileActivityEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ActivityIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid activity, CancellationToken cancellation, TableDbContext db)
        => await db.TFileActivity.AsNoTracking().AnyAsync(x => x.ActivityIdentifier == activity, cancellation);
}