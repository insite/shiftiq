using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Metadata;

public class TActionReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly TActionAdapter _adapter;

    public TActionReader(IDbContextFactory<TableDbContext> context, TActionAdapter adapter)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    }

    public async Task<bool> AssertAsync(Guid action, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TAction
            .AnyAsync(x => x.ActionIdentifier == action, cancellation);
    }

    public async Task<IEnumerable<TActionEntity>> DownloadAsync(IActionCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public TActionEntity? Retrieve(Guid action)
    {
        using var db = _context.CreateDbContext();

        return db.TAction
            .AsNoTracking()
            .FirstOrDefault(x => x.ActionIdentifier == action);
    }

    public async Task<TActionEntity?> RetrieveAsync(Guid action, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TAction
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ActionIdentifier == action, cancellation);
    }

    public async Task<TActionEntity?> RetrieveAsync(string actionUrl, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.TAction
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ActionUrl == actionUrl, cancellation);
    }

    public async Task<int> CountAsync(IActionCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<TActionEntity>> CollectAsync(IActionCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<ActionMatch>> SearchAsync(IActionCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<TActionEntity> BuildQueryable(TableDbContext db, IActionCriteria criteria)
    {
        var q = db.TAction.AsNoTracking().AsQueryable();

        if (criteria.NavigationParentActionIdentifier != null)
            q = q.Where(x => x.NavigationParentActionIdentifier == criteria.NavigationParentActionIdentifier);

        if (criteria.PermissionParentActionIdentifier != null)
            q = q.Where(x => x.PermissionParentActionIdentifier == criteria.PermissionParentActionIdentifier);

        if (criteria.ActionIcon != null)
            q = q.Where(x => x.ActionIcon == criteria.ActionIcon);

        if (criteria.ActionList != null)
            q = q.Where(x => x.ActionList == criteria.ActionList);

        if (criteria.ActionName != null)
            q = q.Where(x => x.ActionName == criteria.ActionName);

        if (criteria.ActionNameShort != null)
            q = q.Where(x => x.ActionNameShort == criteria.ActionNameShort);

        if (criteria.ActionType != null)
            q = q.Where(x => x.ActionType == criteria.ActionType);

        if (criteria.ActionUrl != null)
            q = q.Where(x => x.ActionUrl == criteria.ActionUrl);

        if (criteria.ControllerPath != null)
            q = q.Where(x => x.ControllerPath == criteria.ControllerPath);

        if (criteria.HelpUrl != null)
            q = q.Where(x => x.HelpUrl == criteria.HelpUrl);

        if (criteria.AuthorizationRequirement != null)
            q = q.Where(x => x.AuthorizationRequirement == criteria.AuthorizationRequirement);

        if (criteria.AuthorityType != null)
            q = q.Where(x => x.AuthorityType == criteria.AuthorityType);

        if (criteria.ExtraBreadcrumb != null)
            q = q.Where(x => x.ExtraBreadcrumb == criteria.ExtraBreadcrumb);

        return q;
    }

    public static async Task<IEnumerable<ActionMatch>> ToMatchesAsync(
        IQueryable<TActionEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new ActionMatch
            {
                ActionIdentifier = entity.ActionIdentifier
            })
            .ToListAsync(cancellation);

        return matches;
    }
}