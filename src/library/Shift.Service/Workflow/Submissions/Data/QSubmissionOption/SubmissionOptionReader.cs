using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionOptionReader : IEntityReader
{
    private string DefaultSort = "ResponseSessionIdentifier, SurveyOptionIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    public SubmissionOptionReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid responseSession, Guid surveyOption, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);
            
            return query.AnyAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyOptionIdentifier == surveyOption, cancellation);

        }, cancellation);
    }

    public Task<List<SubmissionOptionEntity>> CollectAsync(ISubmissionOptionCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(ISubmissionOptionCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<SubmissionOptionEntity> DownloadAsync(ISubmissionOptionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<SubmissionOptionEntity?> RetrieveAsync(Guid responseSession, Guid surveyOption, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.ResponseSessionIdentifier == responseSession && x.SurveyOptionIdentifier == surveyOption, cancellation);

        }, cancellation);
    }

    public Task<List<SubmissionOptionMatch>> SearchAsync(ISubmissionOptionCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<SubmissionOptionEntity> BuildQueryable(TableDbContext db)
    {
        ValidateOrganizationContext();

        var query = db.SubmissionOption
            .AsNoTracking()
            .Where(x => x.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<SubmissionOptionEntity> BuildQueryable(TableDbContext db, ISubmissionOptionCriteria criteria)
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

    public static async Task<List<SubmissionOptionMatch>> ToMatchesAsync(IQueryable<SubmissionOptionEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new SubmissionOptionMatch
            {
                ResponseSessionIdentifier = entity.ResponseSessionIdentifier,
                SurveyOptionIdentifier = entity.SurveyOptionIdentifier

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