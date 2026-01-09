using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Directory;

public class MembershipWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public MembershipWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(MembershipEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.MembershipIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QMembership.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid membership, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QMembership.SingleOrDefaultAsync(x => x.MembershipIdentifier == membership, cancellation);
        if (entity == null)
            return false;

        db.QMembership.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(MembershipEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.MembershipIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid membership, CancellationToken cancellation, TableDbContext db)
        => await db.QMembership.AsNoTracking().AnyAsync(x => x.MembershipIdentifier == membership, cancellation);
}