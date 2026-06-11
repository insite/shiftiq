using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Security;

public class UserWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public UserWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(UserEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.UserIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QUser.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid user, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QUser.SingleOrDefaultAsync(x => x.UserIdentifier == user, cancellation);
        if (entity == null)
            return false;

        db.QUser.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(UserEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.UserIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid user, CancellationToken cancellation, TableDbContext db)
        => await db.QUser.AsNoTracking().AnyAsync(x => x.UserIdentifier == user, cancellation);
}