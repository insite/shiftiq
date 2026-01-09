using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Assessment;

public class AttemptSectionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public AttemptSectionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(AttemptSectionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.SectionIndex, cancellation, db);
        if (exists)
            return false;
                
        await db.AttemptSection.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> DeleteAsync(Guid attempt, int sectionIndex, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.AttemptSection.SingleOrDefaultAsync(x => x.AttemptIdentifier == attempt && x.SectionIndex == sectionIndex, cancellation);
        if (entity == null)
            return false;

        db.AttemptSection.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }
        
    public async Task<bool> ModifyAsync(AttemptSectionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.AttemptIdentifier, entity.SectionIndex, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid attempt, int sectionIndex, CancellationToken cancellation, TableDbContext db)
		=> await db.AttemptSection.AsNoTracking().AnyAsync(x => x.AttemptIdentifier == attempt && x.SectionIndex == sectionIndex, cancellation);
}