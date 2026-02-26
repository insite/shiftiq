using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Progress;

public class CredentialReader : IEntityReader
{
    private string DefaultSort = "CredentialIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public CredentialReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid credential, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.CredentialIdentifier == credential
                    && (organization == null || organization == x.OrganizationIdentifier),
                cancellation);

        }, cancellation);
    }

    public Task<List<CredentialEntity>> CollectAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<CredentialEntity> DownloadAsync(ICredentialCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<CredentialEntity?> RetrieveAsync(Guid credential, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.CredentialIdentifier == credential, cancellation);

        }, cancellation);
    }

    public Task<List<CredentialMatch>> SearchAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<CredentialEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.QCredential
            .AsNoTracking();

        return query;
    }

    private IQueryable<CredentialEntity> BuildQueryable(TableDbContext db, ICredentialCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.AchievementId.HasValue)
            query = query.Where(x => x.AchievementIdentifier == criteria.AchievementId);

        if (criteria.LearnerId.HasValue)
            query = query.Where(x => x.UserIdentifier == criteria.LearnerId);

        if (criteria.OrganizationId.HasValue)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<CredentialMatch>> ToMatchesAsync(IQueryable<CredentialEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new CredentialMatch
            {
                AchievementId = entity.AchievementIdentifier,

                LearnerId = entity.UserIdentifier,

                CredentialId = entity.CredentialIdentifier,
                CredentialIssued = entity.CredentialGranted,
                CredentialStatus = entity.CredentialStatus,
                CredentialNecessity = entity.CredentialNecessity,
                CredentialIsRequired = entity.CredentialNecessity == "Mandatory"
            })
            .ToListAsync(cancellation);

        return matches;
    }
}