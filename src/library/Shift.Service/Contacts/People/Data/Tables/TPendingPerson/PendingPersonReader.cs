using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Directory;

public class PendingPersonReader : IEntityReader
{
    private string DefaultEntitySort = "SubmittedAt DESC";
 
    private readonly IDbContextFactory<TableDbContext> _context;

    public PendingPersonReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid pending, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db);

            return query.AnyAsync(x => x.PendingPersonIdentifier == pending
                    && (organization == null || organization == x.OrganizationIdentifier),
                cancellation);

        }, cancellation);
    }

    public Task<List<PendingPersonEntity>> CollectAsync(IPendingPersonCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultEntitySort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<PendingPersonImportModel[]> CollectImportAsync(IPendingPersonCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultEntitySort)
                .ApplyPaging(criteria.Filter)
                .Select(x => new PendingPersonImportModel
                {
                    PendingPersonId = x.PendingPersonIdentifier,
                    PersonCode = x.PersonCode,
                    UserEmail = x.UserEmail,
                    UserFirstName = x.UserFirstName,
                    UserLastName = x.UserLastName,
                    EmployeeStatus = x.EmployeeStatus,
                    MatchCount = db.QPerson
                        .Where(y =>
                            y.OrganizationIdentifier == x.OrganizationIdentifier
                            && y.User!.FirstName == x.UserFirstName
                            && y.User!.LastName == x.UserLastName
                        )
                        .Count()
                })
                .ToArrayAsync(cancellation);
        }, cancellation);
    }

    public Task<int> CountAsync(IPendingPersonCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<PendingPersonEntity> DownloadAsync(IPendingPersonCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildEntityQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<PendingPersonEntity?> RetrieveAsync(Guid pending, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db);

            return query.FirstOrDefaultAsync(x => x.PendingPersonIdentifier == pending, cancellation);

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
    private IQueryable<PendingPersonEntity> BuildEntityQueryable(TableDbContext db)
    {
        var query = db.PendingPerson
            .AsNoTracking();

        return query;
    }

    private IQueryable<PendingPersonEntity> BuildEntityQueryable(TableDbContext db, IPendingPersonCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildEntityQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.SubmittedBy != null)
            query = query.Where(x => x.SubmittedBy == criteria.SubmittedBy);

        if (criteria.SubmittedSince != null)
            query = query.Where(x => x.SubmittedAt >= criteria.SubmittedSince);

        if (criteria.SubmittedBefore != null)
            query = query.Where(x => x.SubmittedAt < criteria.SubmittedBefore);

        if (!string.IsNullOrEmpty(criteria.PersonCode))
            query = query.Where(x => x.PersonCode.Contains(criteria.PersonCode));

        if (!string.IsNullOrEmpty(criteria.UserEmail))
            query = query.Where(x => x.UserEmail.Contains(criteria.UserEmail));

        if (!string.IsNullOrEmpty(criteria.UserFirstName))
            query = query.Where(x => x.UserFirstName.Contains(criteria.UserFirstName));

        if (!string.IsNullOrEmpty(criteria.UserLastName))
            query = query.Where(x => x.UserLastName.Contains(criteria.UserLastName));

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }
}
