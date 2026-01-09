using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class AttemptOptionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public AttemptOptionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(AttemptOptionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.OptionKey, entity.QuestionIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.AttemptOption.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid attempt, int optionKey, Guid question, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.AttemptOption.SingleOrDefaultAsync(x => x.AttemptIdentifier == attempt && x.OptionKey == optionKey && x.QuestionIdentifier == question, cancellation);
        if (entity == null)
            return false;

        db.AttemptOption.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(AttemptOptionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.OptionKey, entity.QuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid attempt, int optionKey, Guid question, CancellationToken cancellation, TableDbContext db)
        => await db.AttemptOption.AsNoTracking().AnyAsync(x => x.AttemptIdentifier == attempt && x.OptionKey == optionKey && x.QuestionIdentifier == question, cancellation);
}