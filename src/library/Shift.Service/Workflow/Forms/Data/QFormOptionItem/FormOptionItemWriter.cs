using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class FormOptionItemWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FormOptionItemWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FormOptionItemEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyOptionItemIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.FormOptionItem.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid surveyOptionItem, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.FormOptionItem.SingleOrDefaultAsync(x => x.SurveyOptionItemIdentifier == surveyOptionItem, cancellation);
        if (entity == null)
            return false;

        db.FormOptionItem.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(FormOptionItemEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyOptionItemIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid surveyOptionItem, CancellationToken cancellation, TableDbContext db)
        => await db.FormOptionItem.AsNoTracking().AnyAsync(x => x.SurveyOptionItemIdentifier == surveyOptionItem, cancellation);
}