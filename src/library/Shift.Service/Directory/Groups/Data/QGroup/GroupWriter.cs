using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Directory;

public class GroupWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public GroupWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(GroupEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.GroupIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QGroup.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid group, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QGroup.SingleOrDefaultAsync(x => x.GroupIdentifier == group, cancellation);
        if (entity == null)
            return false;

        db.QGroup.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(GroupEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.GroupIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid group, CancellationToken cancellation, TableDbContext db)
        => await db.QGroup.AsNoTracking().AnyAsync(x => x.GroupIdentifier == group, cancellation);
}