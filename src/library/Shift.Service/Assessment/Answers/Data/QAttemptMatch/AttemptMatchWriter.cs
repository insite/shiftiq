using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class AttemptMatchWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public AttemptMatchWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(AttemptMatchEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.MatchSequence, entity.QuestionIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.AttemptMatch.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid attempt, int matchSequence, Guid question, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.AttemptMatch.SingleOrDefaultAsync(x => x.AttemptIdentifier == attempt && x.MatchSequence == matchSequence && x.QuestionIdentifier == question, cancellation);
        if (entity == null)
            return false;

        db.AttemptMatch.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(AttemptMatchEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.MatchSequence, entity.QuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid attempt, int matchSequence, Guid question, CancellationToken cancellation, TableDbContext db)
        => await db.AttemptMatch.AsNoTracking().AnyAsync(x => x.AttemptIdentifier == attempt && x.MatchSequence == matchSequence && x.QuestionIdentifier == question, cancellation);
}