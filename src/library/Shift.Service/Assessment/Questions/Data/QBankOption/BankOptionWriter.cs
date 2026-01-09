using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankOptionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public BankOptionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(BankOptionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.OptionKey, entity.QuestionIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.BankOption.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(int optionKey, Guid question, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.BankOption.SingleOrDefaultAsync(x => x.OptionKey == optionKey && x.QuestionIdentifier == question, cancellation);
        if (entity == null)
            return false;

        db.BankOption.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(BankOptionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.OptionKey, entity.QuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(int optionKey, Guid question, CancellationToken cancellation, TableDbContext db)
		=> await db.BankOption.AsNoTracking().AnyAsync(x => x.OptionKey == optionKey && x.QuestionIdentifier == question, cancellation);
}