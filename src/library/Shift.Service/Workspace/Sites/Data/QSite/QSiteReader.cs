using Microsoft.EntityFrameworkCore;

using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Site;

public class QSiteReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly QSiteAdapter _adapter;

    public QSiteReader(IDbContextFactory<TableDbContext> context, QSiteAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    public async Task<bool> AssertAsync(Guid site, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QSite
            .AnyAsync(x => x.SiteIdentifier == site, cancellation);
    }

    public async Task<QSiteEntity?> RetrieveAsync(Guid site, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QSite
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SiteIdentifier == site, cancellation);
    }

    public async Task<int> CountAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQuery(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QSiteEntity>> CollectAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQuery(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<SiteMatch>> SearchAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var entities = await BuildQuery(db, criteria)
            .ToListAsync(cancellation);

        return _adapter.ToMatch(entities);
    }

    private IQueryable<QSiteEntity> BuildQuery(TableDbContext db, ISiteCriteria criteria)
    {
        var query = db.QSite.AsNoTracking().AsQueryable();

        // TODO: Implement search criteria

        // if (criteria.SiteIdentifier != null)
        //    query = query.Where(x => x.SiteIdentifier == criteria.SiteIdentifier);

        // if (criteria.SiteDomain != null)
        //    query = query.Where(x => x.SiteDomain == criteria.SiteDomain);

        // if (criteria.SiteTitle != null)
        //    query = query.Where(x => x.SiteTitle == criteria.SiteTitle);

        // if (criteria.OrganizationIdentifier != null)
        //    query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);

        // if (criteria.LastChangeTime != null)
        //    query = query.Where(x => x.LastChangeTime == criteria.LastChangeTime);

        // if (criteria.LastChangeType != null)
        //    query = query.Where(x => x.LastChangeType == criteria.LastChangeType);

        // if (criteria.LastChangeUser != null)
        //    query = query.Where(x => x.LastChangeUser == criteria.LastChangeUser);

        if (criteria.Filter != null)
        {
            query = query
                .Skip((criteria.Filter.Page - 1) * criteria.Filter.PageSize)
                .Take(criteria.Filter.PageSize);
        }

        return query;
    }
}