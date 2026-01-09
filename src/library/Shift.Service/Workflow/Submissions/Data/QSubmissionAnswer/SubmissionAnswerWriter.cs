using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class SubmissionAnswerWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public SubmissionAnswerWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(SubmissionAnswerEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ResponseSessionIdentifier, entity.SurveyQuestionIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.SubmissionAnswer.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid responseSession, Guid surveyQuestion, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.SubmissionAnswer.SingleOrDefaultAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyQuestionIdentifier == surveyQuestion, cancellation);
        if (entity == null)
            return false;

        db.SubmissionAnswer.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(SubmissionAnswerEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ResponseSessionIdentifier, entity.SurveyQuestionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid responseSession, Guid surveyQuestion, CancellationToken cancellation, TableDbContext db)
        => await db.SubmissionAnswer.AsNoTracking().AnyAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyQuestionIdentifier == surveyQuestion, cancellation);
}