using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class FormOptionListWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FormOptionListWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FormOptionListEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyOptionListIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.FormOptionList.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid surveyOptionList, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.FormOptionList.SingleOrDefaultAsync(x => x.SurveyOptionListIdentifier == surveyOptionList, cancellation);
        if (entity == null)
            return false;

        db.FormOptionList.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(FormOptionListEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyOptionListIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid surveyOptionList, CancellationToken cancellation, TableDbContext db)
        => await db.FormOptionList.AsNoTracking().AnyAsync(x => x.SurveyOptionListIdentifier == surveyOptionList, cancellation);
}