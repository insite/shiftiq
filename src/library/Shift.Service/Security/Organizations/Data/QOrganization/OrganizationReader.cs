using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Security;

public class OrganizationReader : IEntityReader
{
    private string DefaultSort = "OrganizationIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public OrganizationReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.OrganizationIdentifier == organization, cancellation);

        }, cancellation);
    }

    public Task<List<OrganizationEntity>> CollectAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<OrganizationEntity> DownloadAsync(IOrganizationCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<OrganizationEntity?> RetrieveAsync(Guid organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.OrganizationIdentifier == organization, cancellation);

        }, cancellation);
    }

    public Task<List<OrganizationMatch>> SearchAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<OrganizationEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.Organization
            .AsNoTracking()
            .Where(x => x.OrganizationCode != null)
            ;

        return query;
    }

    private IQueryable<OrganizationEntity> BuildQueryable(TableDbContext db, IOrganizationCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.CompanyNameContains.IsNotEmpty())
            query = query.Where(x => x.CompanyName!.Contains(criteria.CompanyNameContains));

        if (criteria.OrganizationCode.IsNotEmpty())
            query = query.Where(x => x.OrganizationCode == criteria.OrganizationCode);

        if (criteria.Filter?.Sort.NullIfEmpty() == null)
            query = query.OrderBy(x => x.CompanyName);
        else
            query = query.OrderBy(criteria.Filter.Sort);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<OrganizationMatch>> ToMatchesAsync(IQueryable<OrganizationEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new OrganizationMatch
            {
                OrganizationIdentifier = entity.OrganizationIdentifier,
                ParentOrganizationIdentifier = entity.ParentOrganizationIdentifier,
                CompanyName = entity.CompanyName
            })
            .ToListAsync(cancellation);

        return matches;
    }
}