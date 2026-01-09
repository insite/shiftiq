using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankFormWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public BankFormWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(BankFormEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FormIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.BankForm.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid form, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.BankForm.SingleOrDefaultAsync(x => x.FormIdentifier == form, cancellation);
        if (entity == null)
            return false;

        db.BankForm.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(BankFormEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FormIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid form, CancellationToken cancellation, TableDbContext db)
		=> await db.BankForm.AsNoTracking().AnyAsync(x => x.FormIdentifier == form, cancellation);
}