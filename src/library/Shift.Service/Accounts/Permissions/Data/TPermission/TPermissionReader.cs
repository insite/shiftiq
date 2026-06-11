using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Security;

public class TPermissionReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public TPermissionReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(
        Guid permission,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TPermission
            .AnyAsync(x => x.PermissionIdentifier == permission, cancellation);
    }

    public async Task<TPermissionEntity?> RetrieveAsync(
        Guid permission,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TPermission
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Organization)
            .FirstOrDefaultAsync(x => x.PermissionIdentifier == permission, cancellation);
    }

    public async Task<int> CountAsync(
        IPermissionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<TPermissionEntity>> CollectAsync(
        IPermissionCriteria criteria,
        CancellationToken cancellation = default)
    {
        var sort = criteria.Filter.Sort ?? "PermissionIdentifier";

        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<TPermissionEntity>> DownloadAsync(
        IPermissionCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PermissionMatch>> SearchAsync(
        IPermissionCriteria criteria,
        CancellationToken cancellation = default)
    {
        var sort = criteria.Filter.Sort ?? "PermissionIdentifier";

        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<TPermissionEntity> BuildQueryable(
        TableDbContext db,
        IPermissionCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = db.TPermission
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Organization)
            .AsQueryable();

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.PermissionGrantedSince.HasValue)
            query = query.Where(x => x.PermissionGranted >= criteria.PermissionGrantedSince);

        if (criteria.PermissionGrantedBefore.HasValue)
            query = query.Where(x => x.PermissionGranted < criteria.PermissionGrantedBefore);

        return query;
    }

    public static async Task<IEnumerable<PermissionMatch>> ToMatchesAsync(
        IQueryable<TPermissionEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PermissionMatch
            {
                PermissionId = entity.PermissionIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}