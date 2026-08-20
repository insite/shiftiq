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
            var now = DateTimeOffset.UtcNow;
            var query = BuildQueryable(db, criteria);

            return query
                .GroupJoin(db.QPerson,
                    credential => new { credential.OrganizationIdentifier, credential.UserIdentifier },
                    person => new { person.OrganizationIdentifier, person.UserIdentifier },
                    (credential, people) => new { Credential = credential, People = people }
                )
                .SelectMany(
                    x => x.People.DefaultIfEmpty(),
                    (x, person) => new CredentialMatch
                    {
                        AchievementId = x.Credential.AchievementIdentifier,
                        AchievementLabel = x.Credential.Achievement!.AchievementLabel,
                        AchievementTitle = x.Credential.Achievement.AchievementTitle,

                        AchievementEffectiveDate = x.Credential.CredentialGranted,
                        AchievementExpiryDate = x.Credential.CredentialExpirationExpected,
                        AchievementValid =
                            x.Credential.CredentialGranted != null
                            && x.Credential.CredentialRevoked == null
                            && (x.Credential.CredentialExpirationExpected == null
                                || x.Credential.CredentialExpirationExpected >= now),

                        CredentialId = x.Credential.CredentialIdentifier,
                        CredentialIssued = x.Credential.CredentialGranted,
                        CredentialStatus = x.Credential.CredentialStatus,
                        CredentialNecessity = x.Credential.CredentialNecessity,
                        CredentialIsRequired = x.Credential.CredentialNecessity == "Mandatory",

                        UserId = x.Credential.UserIdentifier,
                        PersonCode = person!.PersonCode
                    }
                )
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);
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

        if (criteria.OrganizationId.HasValue)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.AchievementId.HasValue)
            query = query.Where(x => x.AchievementIdentifier == criteria.AchievementId);

        if (criteria.UserId.HasValue)
            query = query.Where(x => x.UserIdentifier == criteria.UserId);

        if (criteria.ModifiedFrom.HasValue)
            query = query.Where(x => x.CredentialModified >= criteria.ModifiedFrom.Value);

        if (criteria.ModifiedBefore.HasValue)
            query = query.Where(x => x.CredentialModified < criteria.ModifiedBefore.Value);

        if (criteria.PersonCode.IsNotEmpty())
            query = query.Where(c => c.User!.People.Any(p => p.OrganizationIdentifier == c.OrganizationIdentifier
                                                          && p.PersonCode == criteria.PersonCode));

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }
}
