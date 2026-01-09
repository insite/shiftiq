using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public BankWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(BankEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.BankIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.Bank.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid bank, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.Bank.SingleOrDefaultAsync(x => x.BankIdentifier == bank, cancellation);
        if (entity == null)
            return false;

        db.Bank.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(BankEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.BankIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid bank, CancellationToken cancellation, TableDbContext db)
		=> await db.Bank.AsNoTracking().AnyAsync(x => x.BankIdentifier == bank, cancellation);
}