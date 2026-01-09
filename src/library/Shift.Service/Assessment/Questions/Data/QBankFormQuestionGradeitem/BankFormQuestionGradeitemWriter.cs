using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class BankFormQuestionGradeitemWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public BankFormQuestionGradeitemWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(BankFormQuestionGradeitemEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FormIdentifier, entity.QuestionIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.BankFormQuestionGradeitem.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid form, Guid question, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.BankFormQuestionGradeitem.SingleOrDefaultAsync(x => x.FormIdentifier == form && x.QuestionIdentifier == question, cancellation);
        if (entity == null)
            return false;

        db.BankFormQuestionGradeitem.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(BankFormQuestionGradeitemEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.FormIdentifier, entity.QuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid form, Guid question, CancellationToken cancellation, TableDbContext db)
		=> await db.BankFormQuestionGradeitem.AsNoTracking().AnyAsync(x => x.FormIdentifier == form && x.QuestionIdentifier == question, cancellation);
}