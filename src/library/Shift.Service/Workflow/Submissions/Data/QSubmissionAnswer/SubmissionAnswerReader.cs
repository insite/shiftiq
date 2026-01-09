using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionAnswerReader : IEntityReader
{
    private string DefaultSort = "ResponseSessionIdentifier, SurveyQuestionIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    public SubmissionAnswerReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid responseSession, Guid surveyQuestion, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);
            
            return query.AnyAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyQuestionIdentifier == surveyQuestion, cancellation);

        }, cancellation);
    }

    public Task<List<SubmissionAnswerEntity>> CollectAsync(ISubmissionAnswerCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(ISubmissionAnswerCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<SubmissionAnswerEntity> DownloadAsync(ISubmissionAnswerCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<SubmissionAnswerEntity?> RetrieveAsync(Guid responseSession, Guid surveyQuestion, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyQuestionIdentifier == surveyQuestion, cancellation);

        }, cancellation);
    }

    public Task<List<SubmissionAnswerMatch>> SearchAsync(ISubmissionAnswerCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            query = query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter);

            return ToMatchesAsync(query, cancellation);

        }, cancellation);
    }

    /// <summary>
    /// Creates a queryable for events
    /// </summary>
    /// <remarks>
    /// If you call .Include() on the DbSet then remember to use .AsSplitQuery() so that cartesian explosion is avoided.
    /// When using split queries with Skip/Take on EF versions prior to 10, pay special attention to make your query
    /// ordering fully unique, otherwise the result set is non-deterministic.
    /// </remarks>
    private IQueryable<SubmissionAnswerEntity> BuildQueryable(TableDbContext db)
    {
        ValidateOrganizationContext();

        var query = db.SubmissionAnswer
            .AsNoTracking()
            .Where(x => x.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<SubmissionAnswerEntity> BuildQueryable(TableDbContext db, ISubmissionAnswerCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        // TODO: Apply criteria

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<SubmissionAnswerMatch>> ToMatchesAsync(IQueryable<SubmissionAnswerEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new SubmissionAnswerMatch
            {
                ResponseSessionIdentifier = entity.ResponseSessionIdentifier,
                SurveyQuestionIdentifier = entity.SurveyQuestionIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }

    private void ValidateOrganizationContext()
    {
        if (_auth.OrganizationId == Guid.Empty)
            throw new InvalidOperationException("Organization context is required");
    }
}