using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;


using Shift.Common;

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
        var q = db.TPermission
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Organization)
            .AsQueryable();

        // FIXME: Implement the logic for this criteria. (This code was auto-generated as a reminder only.)

        // TODO: Implement search criteria

        // if (criteria.AllowExecute != null)
        //    query = query.Where(x => x.AllowExecute == criteria.AllowExecute);

        // if (criteria.AllowRead != null)
        //    query = query.Where(x => x.AllowRead == criteria.AllowRead);

        // if (criteria.AllowWrite != null)
        //    query = query.Where(x => x.AllowWrite == criteria.AllowWrite);

        // if (criteria.AllowCreate != null)
        //    query = query.Where(x => x.AllowCreate == criteria.AllowCreate);

        // if (criteria.AllowDelete != null)
        //    query = query.Where(x => x.AllowDelete == criteria.AllowDelete);

        // if (criteria.AllowAdministrate != null)
        //    query = query.Where(x => x.AllowAdministrate == criteria.AllowAdministrate);

        // if (criteria.AllowConfigure != null)
        //    query = query.Where(x => x.AllowConfigure == criteria.AllowConfigure);

        // if (criteria.PermissionMask != null)
        //    query = query.Where(x => x.PermissionMask == criteria.PermissionMask);

        // if (criteria.PermissionGranted != null)
        //    query = query.Where(x => x.PermissionGranted == criteria.PermissionGranted);

        // if (criteria.PermissionGrantedBy != null)
        //    query = query.Where(x => x.PermissionGrantedBy == criteria.PermissionGrantedBy);

        // if (criteria.ObjectIdentifier != null)
        //    query = query.Where(x => x.ObjectIdentifier == criteria.ObjectIdentifier);

        // if (criteria.ObjectType != null)
        //    query = query.Where(x => x.ObjectType == criteria.ObjectType);

        // if (criteria.GroupIdentifier != null)
        //    query = query.Where(x => x.GroupIdentifier == criteria.GroupIdentifier);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.PermissionIdentifier != null)
        //    query = query.Where(x => x.PermissionIdentifier == criteria.PermissionIdentifier);

        // if (criteria.AllowTrialAccess != null)
        //    query = query.Where(x => x.AllowTrialAccess == criteria.AllowTrialAccess);

        return q;
    }

    public static async Task<IEnumerable<PermissionMatch>> ToMatchesAsync(
        IQueryable<TPermissionEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PermissionMatch
            {
                PermissionIdentifier = entity.PermissionIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}