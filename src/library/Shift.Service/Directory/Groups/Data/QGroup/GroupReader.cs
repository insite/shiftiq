using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Directory;

public class GroupReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    private string DefaultSort = "GroupIdentifier";

    public GroupReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid group, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.GroupIdentifier == group, cancellation);

        }, cancellation);
    }

    public Task<List<GroupEntity>> CollectAsync(IGroupCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IGroupCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<GroupEntity> DownloadAsync(IGroupCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<GroupEntity?> RetrieveAsync(Guid group, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.GroupIdentifier == group, cancellation);

        }, cancellation);
    }

    public Task<List<GroupMatch>> SearchAsync(IGroupCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<GroupEntity> BuildQueryable(TableDbContext db)
    {
        ValidateOrganizationContext();

        var query = db.QGroup
            .AsNoTracking()
            .Where(x => x.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<GroupEntity> BuildQueryable(TableDbContext db, IGroupCriteria criteria)
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

    public static async Task<List<GroupMatch>> ToMatchesAsync(IQueryable<GroupEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new GroupMatch
            {
                GroupIdentifier = entity.GroupIdentifier,
                GroupName = entity.GroupName
            })
            .ToListAsync(cancellation);

        return matches;
    }

    private void ValidateOrganizationContext()
    {
        if (_auth.OrganizationId == Guid.Empty)
            throw new InvalidOperationException("Organization context is required");
    }

    public async Task<string[]> SearchUserRolesAsync(Guid? parentOrganizationId, Guid organizationId, Guid userId)
    {
        using var db = _context.CreateDbContext();

        return await db.QMembership.Where(m => m.UserIdentifier == userId)
            .Join(
                db.QGroup
                    .Where(g =>
                        (g.OrganizationIdentifier == organizationId || (parentOrganizationId != null && g.OrganizationIdentifier == parentOrganizationId))
                        && g.GroupType == "Role"
                    ),
                m => m.GroupIdentifier,
                g => g.GroupIdentifier,
                (m, g) => g.GroupName
            )
            .ToArrayAsync();
    }
}

public interface IGroupLookupService
{
    Task<IReadOnlyDictionary<Guid, string>> GetGroupNamesAsync(IEnumerable<Guid> groupIds);
}

public class GroupLookupService : IGroupLookupService
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public GroupLookupService(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetGroupNamesAsync(IEnumerable<Guid> groupIds)
    {
        var ids = groupIds.ToList();

        if (ids.Count == 0)
            return new Dictionary<Guid, string>();

        using var db = _context.CreateDbContext();

        return await db.QGroup
            .AsNoTracking()
            .Where(g => ids.Contains(g.GroupIdentifier))
            .ToDictionaryAsync(
                g => g.GroupIdentifier,
                g => g.GroupName);
    }
}