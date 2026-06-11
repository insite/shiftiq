using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Security;

public class UserReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private string DefaultSort = "FullName, UserIdentifier";

    public UserReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid user, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, organization);

            return query.AnyAsync(x => x.UserIdentifier == user, cancellation);

        }, cancellation);
    }

    public Task<List<UserEntity>> CollectAsync(IUserCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IUserCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<UserEntity> DownloadAsync(IUserCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<UserEntity?> RetrieveAsync(Guid user, Guid? organization = null, CancellationToken cancellation = default)
    {
        if (user == Guid.Empty)
            return Task.FromResult<UserEntity?>(null);

        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, organization);

            return query.FirstOrDefaultAsync(x => x.UserIdentifier == user, cancellation);

        }, cancellation);
    }

    public Task<List<UserMatch>> SearchAsync(IUserCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<UserEntity> BuildQueryable(TableDbContext db, Guid? organization)
    {
        var query = db.QUser.AsNoTracking();

        if (organization != null)
        {
            var organizationUsers = db.QPerson
                .Where(x => x.OrganizationIdentifier == organization.Value)
                .Select(x => x.UserIdentifier);

            query = query
                .Join(organizationUsers, u => u.UserIdentifier, user => user, (u, user) => u);
        }

        return query;
    }

    private IQueryable<UserEntity> BuildQueryable(TableDbContext db, IUserCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db, criteria.OrganizationId);

        if (criteria.UserIds != null && criteria.UserIds.Length > 0)
            query = query.Where(x => criteria.UserIds.Contains(x.UserIdentifier));

        if (criteria.UserEmailExact.IsNotEmpty())
            query = query.Where(x => x.Email == criteria.UserEmailExact);

        if (criteria.UserFullNameContains.IsNotEmpty())
            query = query.Where(x => x.FullName.Contains(criteria.UserFullNameContains));

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<UserMatch>> ToMatchesAsync(IQueryable<UserEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new UserMatch
            {
                UserId = entity.UserIdentifier,
                FullName = entity.FullName
            })
            .ToListAsync(cancellation);

        return matches;
    }

}
