using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Directory;

public class PersonReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private string DefaultSort = "User.FullName, PersonIdentifier";

    public PersonReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid person, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            if (organization != null)
                query = query.Where(x => x.OrganizationIdentifier == organization.Value);

            var exists = query.AnyAsync(x => x.PersonIdentifier == person, cancellation);

            return exists;

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
            var query = BuildQueryable(db);

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
    private IQueryable<PersonEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.QPerson
            .Include(x => x.User)
            .Include(x => x.BillingAddress)
            .Include(x => x.HomeAddress)
            .Include(x => x.ShippingAddress)
            .Include(x => x.WorkAddress)
            .AsNoTracking();

        return query;
    }

    private IQueryable<PersonEntity> BuildQueryable(TableDbContext db, IPersonCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (!string.IsNullOrEmpty(criteria.EmailExact))
            query = query.Where(x => x.User!.Email == criteria.EmailExact);

        if (criteria.EventRole != null)
            query = query.Where(x => x.User!.Events.Any(e => e.OrganizationIdentifier == x.OrganizationIdentifier && e.AttendeeRole == criteria.EventRole));

        if (!string.IsNullOrEmpty(criteria.FullName))
            query = query.Where(x => x.User!.FullName.Contains(criteria.FullName));

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.UserId != null)
            query = query.Where(x => x.UserIdentifier == criteria.UserId);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<PersonMatch>> ToMatchesAsync(IQueryable<PersonEntity> queryable, CancellationToken cancellation = default)
    {
        var adapter = new PersonAdapter();

        var matches = await queryable
            .Select(entity => adapter.ToMatch(entity))
            .ToListAsync(cancellation);

        return matches;
    }
}