using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Security;

public class UserConnectionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public UserConnectionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(UserConnectionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FromUserIdentifier, entity.ToUserIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QUserConnection.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid fromUser, Guid toUser, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QUserConnection.SingleOrDefaultAsync(x => x.FromUserIdentifier == fromUser && x.ToUserIdentifier == toUser, cancellation);
        if (entity == null)
            return false;

        db.QUserConnection.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(UserConnectionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FromUserIdentifier, entity.ToUserIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid fromUser, Guid toUser, CancellationToken cancellation, TableDbContext db)
        => await db.QUserConnection.AsNoTracking().AnyAsync(x => x.FromUserIdentifier == fromUser && x.ToUserIdentifier == toUser, cancellation);
}