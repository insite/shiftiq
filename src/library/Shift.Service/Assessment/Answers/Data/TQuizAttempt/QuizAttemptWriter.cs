using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class QuizAttemptWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QuizAttemptWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(QuizAttemptEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.QuizAttempt.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid attempt, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.QuizAttempt.SingleOrDefaultAsync(x => x.AttemptIdentifier == attempt, cancellation);
        if (entity == null)
            return false;

        db.QuizAttempt.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(QuizAttemptEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid attempt, CancellationToken cancellation, TableDbContext db)
		=> await db.QuizAttempt.AsNoTracking().AnyAsync(x => x.AttemptIdentifier == attempt, cancellation);
}