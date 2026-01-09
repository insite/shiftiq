using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class FormWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FormWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FormEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyFormIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.Form.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid surveyForm, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.Form.SingleOrDefaultAsync(x => x.SurveyFormIdentifier == surveyForm, cancellation);
        if (entity == null)
            return false;

        db.Form.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(FormEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyFormIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid surveyForm, CancellationToken cancellation, TableDbContext db)
        => await db.Form.AsNoTracking().AnyAsync(x => x.SurveyFormIdentifier == surveyForm, cancellation);
}