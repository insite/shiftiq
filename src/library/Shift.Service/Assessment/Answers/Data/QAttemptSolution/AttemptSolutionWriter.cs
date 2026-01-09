using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class AttemptSolutionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public AttemptSolutionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(AttemptSolutionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.QuestionIdentifier, entity.SolutionIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.AttemptSolution.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid attempt, Guid question, Guid solution, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.AttemptSolution.SingleOrDefaultAsync(x => x.AttemptIdentifier == attempt && x.QuestionIdentifier == question && x.SolutionIdentifier == solution, cancellation);
        if (entity == null)
            return false;

        db.AttemptSolution.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(AttemptSolutionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.QuestionIdentifier, entity.SolutionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid attempt, Guid question, Guid solution, CancellationToken cancellation, TableDbContext db)
		=> await db.AttemptSolution.AsNoTracking().AnyAsync(x => x.AttemptIdentifier == attempt && x.QuestionIdentifier == question && x.SolutionIdentifier == solution, cancellation);
}