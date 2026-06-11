using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class CaseUserWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public CaseUserWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(CaseUserEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.JoinIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QCaseUser.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid join, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QCaseUser.SingleOrDefaultAsync(x => x.JoinIdentifier == join, cancellation);
        if (entity == null)
            return false;

        db.QCaseUser.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(CaseUserEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.JoinIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid join, CancellationToken cancellation, TableDbContext db)
		=> await db.QCaseUser.AsNoTracking().AnyAsync(x => x.JoinIdentifier == join, cancellation);
}