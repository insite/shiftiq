using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class CaseGroupWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public CaseGroupWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(CaseGroupEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.JoinIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QCaseGroup.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid join, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QCaseGroup.SingleOrDefaultAsync(x => x.JoinIdentifier == join, cancellation);
        if (entity == null)
            return false;

        db.QCaseGroup.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(CaseGroupEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.JoinIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid join, CancellationToken cancellation, TableDbContext db)
		=> await db.QCaseGroup.AsNoTracking().AnyAsync(x => x.JoinIdentifier == join, cancellation);
}