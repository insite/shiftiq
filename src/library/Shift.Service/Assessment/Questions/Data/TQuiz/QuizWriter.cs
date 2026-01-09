using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class QuizWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QuizWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(QuizEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.QuizIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.Quiz.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid quiz, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.Quiz.SingleOrDefaultAsync(x => x.QuizIdentifier == quiz, cancellation);
        if (entity == null)
            return false;

        db.Quiz.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(QuizEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.QuizIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid quiz, CancellationToken cancellation, TableDbContext db)
		=> await db.Quiz.AsNoTracking().AnyAsync(x => x.QuizIdentifier == quiz, cancellation);
}