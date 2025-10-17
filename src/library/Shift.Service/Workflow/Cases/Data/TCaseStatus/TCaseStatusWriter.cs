using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Cases;

public class TCaseStatusWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly TCaseStatusAdapter _adapter = new();
    private readonly IShiftIdentityService _identity;

    public TCaseStatusWriter(IDbContextFactory<TableDbContext> context, IShiftIdentityService identity)
    {
        _context = context;
        _identity = identity;
    }

    public async Task<TCaseStatusEntity?> CreateAsync(CreateCaseStatus command, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(command);
        entity.StatusIdentifier = UniqueIdentifier.Create();
        entity.OrganizationIdentifier = _identity.OrganizationId;

        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.StatusIdentifier, cancellation, db);
        if (exists)
            return null;

        await db.TCaseStatus.AddAsync(entity, cancellation);

        return await db.SaveChangesAsync(cancellation) > 0 ? entity : null;
    }

    public async Task<bool> ModifyAsync(TCaseStatusEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.StatusIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;

        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid statusId, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.TCaseStatus.SingleOrDefaultAsync(
            x => x.StatusIdentifier == statusId
              && x.OrganizationIdentifier == _identity.OrganizationId,
            cancellation);

        if (entity == null)
            return false;

        db.TCaseStatus.Remove(entity);

        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid statusId, CancellationToken cancellation, TableDbContext db)
        => await db.TCaseStatus.AsNoTracking().AnyAsync(x => x.StatusIdentifier == statusId, cancellation);
}
