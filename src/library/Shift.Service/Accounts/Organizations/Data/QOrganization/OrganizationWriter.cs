using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Security;

public class OrganizationWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public OrganizationWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(OrganizationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.OrganizationIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.Organization.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid organization, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.Organization.SingleOrDefaultAsync(x => x.OrganizationIdentifier == organization, cancellation);
        if (entity == null)
            return false;

        db.Organization.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(OrganizationEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.OrganizationIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid organization, CancellationToken cancellation, TableDbContext db)
        => await db.Organization.AsNoTracking().AnyAsync(x => x.OrganizationIdentifier == organization, cancellation);
}