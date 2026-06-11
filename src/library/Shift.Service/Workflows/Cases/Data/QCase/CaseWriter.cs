using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class CaseWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public CaseWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(CaseEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CaseIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.QCase.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid issue, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QCase.SingleOrDefaultAsync(x => x.CaseIdentifier == issue, cancellation);
        if (entity == null)
            return false;

        db.QCase.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(CaseEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.CaseIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid issue, CancellationToken cancellation, TableDbContext db)
        => await db.QCase.AsNoTracking().AnyAsync(x => x.CaseIdentifier == issue, cancellation);
}