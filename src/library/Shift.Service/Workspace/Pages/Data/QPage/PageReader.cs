using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;
using System.Collections.Generic;

namespace Shift.Service.Workspace;

public class PageReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private string DefaultSort = "PageIdentifier";

    public PageReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid page, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.PageIdentifier == page && (organization == null || x.OrganizationIdentifier == organization), cancellation);

        }, cancellation);
    }

    public Task<List<PageEntity>> CollectAsync(IPageCriteria criteria, CancellationToken cancellation = default)
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

    public Task<List<PageEntity>> CollectChildrenAsync(Guid parentPageId, CancellationToken cancellation = default)
    {
        return ExecuteAsync(async db =>
        {
            var children = await db.QPage.Where(x => x.ParentPageIdentifier == parentPageId).ToListAsync();
            var ids = children.Select(x => x.PageIdentifier).ToHashSet();
            var index = 0;

            while (index < children.Count)
            {
                var subChildren = await db.QPage.Where(x => x.ParentPageIdentifier == children[index].PageIdentifier).ToListAsync();
                foreach (var subChild in subChildren)
                {
                    if (!ids.Contains(subChild.PageIdentifier))
                    {
                        children.Add(subChild);
                        ids.Add(subChild.PageIdentifier);
                    }
                }
                index++;
            }

            return children;
        }, cancellation);        
    }

    public Task<int> CountAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<PageEntity> DownloadAsync(IPageCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<PageEntity?> RetrieveAsync(Guid page, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.PageIdentifier == page, cancellation);

        }, cancellation);
    }

    public Task<List<PageMatch>> SearchAsync(IPageCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<PageEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.QPage.AsNoTracking();

        return query;
    }

    private IQueryable<PageEntity> BuildQueryable(TableDbContext db, IPageCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.PageType != null)
            query = query.Where(x => x.PageType == criteria.PageType);

        if (criteria.Title != null)
            query = query.Where(x => x.Title.Contains(criteria.Title));

        if (criteria.Sequence != null)
            query = query.Where(x => x.Sequence == criteria.Sequence);

        if (criteria.AuthorDate != null)
            query = query.Where(x => x.AuthorDate == criteria.AuthorDate);

        if (criteria.AuthorName != null)
            query = query.Where(x => x.AuthorName == criteria.AuthorName);

        if (criteria.ContentControl != null)
            query = query.Where(x => x.ContentControl == criteria.ContentControl);

        if (criteria.NavigateUrl != null)
            query = query.Where(x => x.NavigateUrl == criteria.NavigateUrl);

        if (criteria.IsHidden != null)
            query = query.Where(x => x.IsHidden == criteria.IsHidden);

        if (criteria.ContentLabels != null)
            query = query.Where(x => x.ContentLabels == criteria.ContentLabels);

        if (criteria.PageIcon != null)
            query = query.Where(x => x.PageIcon == criteria.PageIcon);

        if (criteria.Hook != null)
            query = query.Where(x => x.Hook == criteria.Hook);

        if (criteria.IsNewTab != null)
            query = query.Where(x => x.IsNewTab == criteria.IsNewTab);

        if (criteria.PageSlug != null)
            query = query.Where(x => x.PageSlug == criteria.PageSlug);

        if (criteria.SiteId != null)
            query = query.Where(x => x.SiteIdentifier == criteria.SiteId);

        if (criteria.ParentPageId != null)
            query = query.Where(x => x.ParentPageIdentifier == criteria.ParentPageId);

        if (criteria.LastChangeTime != null)
            query = query.Where(x => x.LastChangeTime == criteria.LastChangeTime);

        if (criteria.LastChangeType != null)
            query = query.Where(x => x.LastChangeType == criteria.LastChangeType);

        if (criteria.LastChangeUser != null)
            query = query.Where(x => x.LastChangeUser == criteria.LastChangeUser);

        if (criteria.IsAccessDenied != null)
            query = query.Where(x => x.IsAccessDenied == criteria.IsAccessDenied);

        if (criteria.ObjectType != null)
            query = query.Where(x => x.ObjectType == criteria.ObjectType);

        if (criteria.ObjectId != null)
            query = query.Where(x => x.ObjectIdentifier == criteria.ObjectId);

        if (criteria.IsNullNavigateUrl != null)
            query = criteria.IsNullNavigateUrl ?? false
                ? query.Where(x => x.NavigateUrl == null)
                : query.Where(x => x.NavigateUrl != null);

        if (criteria.ParentPageSlug != null)
            query = query.Where(x => x.Parent!.PageSlug == criteria.ParentPageSlug);

        if (criteria.ParentPageType != null)
            query = query.Where(x => x.Parent!.PageType == criteria.ParentPageType);

        if (criteria.SiteDomain != null)
            query = query.Where(x => x.Site != null && x.Site.SiteDomain == criteria.SiteDomain);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<PageMatch>> ToMatchesAsync(IQueryable<PageEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PageMatch
            {
                PageId = entity.PageIdentifier,
                PageIcon = entity.PageIcon,
                PageTitle = entity.Title,
                PageUrl = entity.NavigateUrl

            })
            .ToListAsync(cancellation);

        return matches;
    }
}
