using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptSolutionReader : IEntityReader
{
    private string DefaultSort = "AttemptIdentifier, QuestionIdentifier, SolutionIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    public AttemptSolutionReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid attempt, Guid question, Guid solution, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.AttemptIdentifier == attempt && x.QuestionIdentifier == question && x.SolutionIdentifier == solution, cancellation);

        }, cancellation);
    }

    public Task<List<AttemptSolutionEntity>> CollectAsync(IAttemptSolutionCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IAttemptSolutionCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<AttemptSolutionEntity> DownloadAsync(IAttemptSolutionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<AttemptSolutionEntity?> RetrieveAsync(Guid attempt, Guid question, Guid solution, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.AttemptIdentifier == attempt && x.QuestionIdentifier == question && x.SolutionIdentifier == solution, cancellation);

        }, cancellation);
    }

    public Task<List<AttemptSolutionMatch>> SearchAsync(IAttemptSolutionCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<AttemptSolutionEntity> BuildQueryable(TableDbContext db)
    {
        ValidateOrganizationContext();

        var query = db.AttemptSolution
            .AsNoTracking()
            .Where(x => x.Attempt != null && x.Attempt.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<AttemptSolutionEntity> BuildQueryable(TableDbContext db, IAttemptSolutionCriteria criteria)
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

    public static async Task<List<AttemptSolutionMatch>> ToMatchesAsync(IQueryable<AttemptSolutionEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new AttemptSolutionMatch
            {
                AttemptIdentifier = entity.AttemptIdentifier,
                QuestionIdentifier = entity.QuestionIdentifier,
                SolutionIdentifier = entity.SolutionIdentifier

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