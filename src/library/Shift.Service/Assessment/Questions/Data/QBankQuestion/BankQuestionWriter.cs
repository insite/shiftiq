using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankQuestionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public BankQuestionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(BankQuestionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.QuestionIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.BankQuestion.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid question, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.BankQuestion.SingleOrDefaultAsync(x => x.QuestionIdentifier == question, cancellation);
        if (entity == null)
            return false;

        db.BankQuestion.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(BankQuestionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.QuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid question, CancellationToken cancellation, TableDbContext db)
		=> await db.BankQuestion.AsNoTracking().AnyAsync(x => x.QuestionIdentifier == question, cancellation);
}