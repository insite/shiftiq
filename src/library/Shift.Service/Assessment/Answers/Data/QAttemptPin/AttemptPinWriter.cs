using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class AttemptPinWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public AttemptPinWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(AttemptPinEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.PinSequence, entity.QuestionIdentifier, cancellation, db);
        if (exists)
            return false;
                
        await db.AttemptPin.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid attempt, int pinSequence, Guid question, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.AttemptPin.SingleOrDefaultAsync(x => x.AttemptIdentifier == attempt && x.PinSequence == pinSequence && x.QuestionIdentifier == question, cancellation);
        if (entity == null)
            return false;

        db.AttemptPin.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(AttemptPinEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.PinSequence, entity.QuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid attempt, int pinSequence, Guid question, CancellationToken cancellation, TableDbContext db)
		=> await db.AttemptPin.AsNoTracking().AnyAsync(x => x.AttemptIdentifier == attempt && x.PinSequence == pinSequence && x.QuestionIdentifier == question, cancellation);
}