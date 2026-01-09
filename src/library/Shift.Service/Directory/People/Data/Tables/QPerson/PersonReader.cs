using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Directory;

public class PersonReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    private string DefaultSort = "User.FullName, PersonIdentifier";

    public PersonReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid person, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, _auth.OrganizationId);

            return query.AnyAsync(x => x.PersonIdentifier == person, cancellation);

        }, cancellation);
    }

    public Task<List<PersonEntity>> CollectAsync(IPersonCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IPersonCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<PersonEntity> DownloadAsync(IPersonCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<PersonEntity?> RetrieveAsync(Guid person, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, _auth.OrganizationId);

            return query.FirstOrDefaultAsync(x => x.PersonIdentifier == person, cancellation);

        }, cancellation);
    }

    public Task<List<PersonMatch>> SearchAsync(IPersonCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<PersonEntity> BuildQueryable(TableDbContext db, Guid organizationId)
    {
        ValidateOrganizationContext(organizationId);

        var query = db.QPerson
            .AsNoTracking()
            .Where(x => x.OrganizationIdentifier == organizationId);

        return query;
    }

    private IQueryable<PersonEntity> BuildQueryable(TableDbContext db, IPersonCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db, criteria.OrganizationIdentifier ?? _auth.OrganizationId);

        if (!string.IsNullOrEmpty(criteria.EmailExact))
            query = query.Where(x => x.User!.Email == criteria.EmailExact);

        if (criteria.EventRole != null)
            query = query.Where(x => x.User!.Events.Any(e => e.OrganizationIdentifier == x.OrganizationIdentifier && e.AttendeeRole == criteria.EventRole));

        if (!string.IsNullOrEmpty(criteria.FullName))
            query = query.Where(x => x.User!.FullName.Contains(criteria.FullName));

        if (criteria.UserIdentifier != null)
            query = query.Where(x => x.UserIdentifier == criteria.UserIdentifier);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<PersonMatch>> ToMatchesAsync(IQueryable<PersonEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PersonMatch
            {
                PersonId = entity.PersonIdentifier,
                UserId = entity.UserIdentifier,

                UserEmail = entity.User != null ? entity.User.Email : "-",
                UserName = entity.User != null ? entity.User.FullName : "-",

                IsAdministrator = entity.IsAdministrator,
                IsDeveloper = entity.IsDeveloper,
                IsOperator = entity.IsOperator,

                TimeZone = entity.User!.TimeZone
            })
            .ToListAsync(cancellation);

        return matches;
    }

    private void ValidateOrganizationContext(Guid organizationId)
    {
        if (organizationId == Guid.Empty)
            throw new InvalidOperationException("Organization context is required");
    }
}