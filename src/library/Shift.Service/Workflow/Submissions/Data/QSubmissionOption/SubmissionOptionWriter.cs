using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Workflow;

public class SubmissionOptionWriter : IEntityWriter
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public SubmissionOptionWriter(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(SubmissionOptionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ResponseSessionIdentifier, entity.SurveyOptionIdentifier, cancellation, db);
        if (exists)
            return false;

        await db.SubmissionOption.AddAsync(entity, cancellation);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> DeleteAsync(Guid responseSession, Guid surveyOption, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entity = await db.SubmissionOption.SingleOrDefaultAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyOptionIdentifier == surveyOption, cancellation);
        if (entity == null)
            return false;

        db.SubmissionOption.Remove(entity);
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    public async Task<bool> ModifyAsync(SubmissionOptionEntity entity, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var exists = await AssertAsync(entity.ResponseSessionIdentifier, entity.SurveyOptionIdentifier, cancellation, db);
        if (!exists)
            return false;

        db.Entry(entity).State = EntityState.Modified;
        return await db.SaveChangesAsync(cancellation) > 0;
    }

    private async Task<bool> AssertAsync(Guid responseSession, Guid surveyOption, CancellationToken cancellation, TableDbContext db)
        => await db.SubmissionOption.AsNoTracking().AnyAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyOptionIdentifier == surveyOption, cancellation);
}