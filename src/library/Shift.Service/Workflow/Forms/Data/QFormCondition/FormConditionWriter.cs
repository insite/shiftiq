using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class FormConditionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public FormConditionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(FormConditionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.MaskedSurveyQuestionIdentifier, entity.MaskingSurveyOptionItemIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.FormCondition.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid maskedSurveyQuestion, Guid maskingSurveyOptionItem, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.FormCondition.SingleOrDefaultAsync(x => x.MaskedSurveyQuestionIdentifier == maskedSurveyQuestion && x.MaskingSurveyOptionItemIdentifier == maskingSurveyOptionItem, cancellation);
        if (entity == null)
            return false;

        db.FormCondition.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(FormConditionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.MaskedSurveyQuestionIdentifier, entity.MaskingSurveyOptionItemIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid maskedSurveyQuestion, Guid maskingSurveyOptionItem, CancellationToken cancellation, TableDbContext db)
        => await db.FormCondition.AsNoTracking().AnyAsync(x => x.MaskedSurveyQuestionIdentifier == maskedSurveyQuestion && x.MaskingSurveyOptionItemIdentifier == maskingSurveyOptionItem, cancellation);
}