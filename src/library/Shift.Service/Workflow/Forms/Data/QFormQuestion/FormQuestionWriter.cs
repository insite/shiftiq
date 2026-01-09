using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class FormQuestionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FormQuestionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FormQuestionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyQuestionIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.FormQuestion.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid surveyQuestion, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.FormQuestion.SingleOrDefaultAsync(x => x.SurveyQuestionIdentifier == surveyQuestion, cancellation);
        if (entity == null)
            return false;

        db.FormQuestion.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(FormQuestionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.SurveyQuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid surveyQuestion, CancellationToken cancellation, TableDbContext db)
        => await db.FormQuestion.AsNoTracking().AnyAsync(x => x.SurveyQuestionIdentifier == surveyQuestion, cancellation);
}