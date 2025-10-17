using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Site;

public class QPageReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public QPageReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AssertAsync(Guid page, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPage
            .AnyAsync(x => x.PageIdentifier == page, cancellation);
    }

    public async Task<QPageEntity?> RetrieveAsync(Guid page, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QPage
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PageIdentifier == page, cancellation);
    }

    public async Task<int> CountAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QPageEntity>> CollectAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<PageMatch>> SearchAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QPageEntity> BuildQueryable(TableDbContext db, IPageCriteria criteria)
    {
        var q = db.QPage.AsNoTracking().AsQueryable();

        if (criteria.PageType != null)
            q = q.Where(x => x.PageType == criteria.PageType);

        if (criteria.Title != null)
            q = q.Where(x => x.Title.Contains(criteria.Title));

        if (criteria.Sequence != null)
            q = q.Where(x => x.Sequence == criteria.Sequence);

        if (criteria.AuthorDate != null)
            q = q.Where(x => x.AuthorDate == criteria.AuthorDate);

        if (criteria.AuthorName != null)
            q = q.Where(x => x.AuthorName == criteria.AuthorName);

        if (criteria.ContentControl != null)
            q = q.Where(x => x.ContentControl == criteria.ContentControl);

        if (criteria.NavigateUrl != null)
            q = q.Where(x => x.NavigateUrl == criteria.NavigateUrl);

        if (criteria.IsHidden != null)
            q = q.Where(x => x.IsHidden == criteria.IsHidden);

        if (criteria.ContentLabels != null)
            q = q.Where(x => x.ContentLabels == criteria.ContentLabels);

        if (criteria.PageIcon != null)
            q = q.Where(x => x.PageIcon == criteria.PageIcon);

        if (criteria.Hook != null)
            q = q.Where(x => x.Hook == criteria.Hook);

        if (criteria.IsNewTab != null)
            q = q.Where(x => x.IsNewTab == criteria.IsNewTab);

        if (criteria.PageSlug != null)
            q = q.Where(x => x.PageSlug == criteria.PageSlug);

        if (criteria.SiteIdentifier != null)
            q = q.Where(x => x.SiteIdentifier == criteria.SiteIdentifier);

        if (criteria.ParentPageIdentifier != null)
            q = q.Where(x => x.ParentPageIdentifier == criteria.ParentPageIdentifier);

        if (criteria.OrganizationIdentifier != null)
            q = q.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        if (criteria.LastChangeTime != null)
            q = q.Where(x => x.LastChangeTime == criteria.LastChangeTime);

        if (criteria.LastChangeType != null)
            q = q.Where(x => x.LastChangeType == criteria.LastChangeType);

        if (criteria.LastChangeUser != null)
            q = q.Where(x => x.LastChangeUser == criteria.LastChangeUser);

        if (criteria.IsAccessDenied != null)
            q = q.Where(x => x.IsAccessDenied == criteria.IsAccessDenied);

        if (criteria.ObjectType != null)
            q = q.Where(x => x.ObjectType == criteria.ObjectType);

        if (criteria.ObjectIdentifier != null)
            q = q.Where(x => x.ObjectIdentifier == criteria.ObjectIdentifier);

        if (criteria.IsNullNavigateUrl != null)
            q = criteria.IsNullNavigateUrl ?? false
                ? q.Where(x => x.NavigateUrl == null)
                : q.Where(x => x.NavigateUrl != null);

        if (criteria.ParentPageSlug != null)
            q = q.Where(x => x.Parent!.PageSlug == criteria.ParentPageSlug);

        if (criteria.ParentPageType != null)
            q = q.Where(x => x.Parent!.PageType == criteria.ParentPageType);

        if (criteria.SiteDomain != null)
            q = q.Where(x => x.Site != null && x.Site.SiteDomain == criteria.SiteDomain);

        return q;
    }

    public static async Task<IEnumerable<PageMatch>> ToMatchesAsync(
        IQueryable<QPageEntity> queryable,
        CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PageMatch
            {
                PageIdentifier = entity.PageIdentifier,

                PageIcon = entity.PageIcon,
                PageTitle = entity.Title,
                PageUrl = entity.NavigateUrl
            })
            .ToListAsync(cancellation);

        return matches;
    }
}