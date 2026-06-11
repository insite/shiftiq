using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Cases;

public class TCaseStatusWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly TCaseStatusAdapter _adapter = new();

    public TCaseStatusWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<TCaseStatusEntity?> CreateAsync(CreateCaseStatus command, Guid organization, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(command);
        entity.StatusIdentifier = UniqueIdentifier.Create();
        entity.OrganizationIdentifier = organization;

        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.StatusIdentifier, organization, cancellation, db);
        if (exists)
            return null;

        await db.TCaseStatus.AddAsync(entity, cancellation);

        return await db.SaveChangesAsync(cancellation) > 0 ? entity : null;
    }

    public async Task<bool> ModifyAsync(TCaseStatusEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.StatusIdentifier, entity.OrganizationIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;

        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid status, Guid organization, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TCaseStatus.SingleOrDefaultAsync(
            x => x.StatusIdentifier == status
              && x.OrganizationIdentifier == organization,
            cancellation);

        if (entity == null)
            return false;

        db.TCaseStatus.Remove(entity);

        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid status, Guid organization, CancellationToken cancellation, TableDbContext db)
        => await db.TCaseStatus.AsNoTracking().AnyAsync(x => x.StatusIdentifier == status && x.OrganizationIdentifier == organization, cancellation);
}
