using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Security;

public class UserSessionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public UserSessionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(UserSessionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SessionIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.TUserSession.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid session, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TUserSession.SingleOrDefaultAsync(x => x.SessionIdentifier == session, cancellation);
        if (entity == null)
            return false;

        db.TUserSession.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(UserSessionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SessionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid session, CancellationToken cancellation, TableDbContext db)
        => await db.TUserSession.AsNoTracking().AnyAsync(x => x.SessionIdentifier == session, cancellation);
}