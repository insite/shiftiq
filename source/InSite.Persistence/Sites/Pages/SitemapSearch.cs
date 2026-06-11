using System;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class SitemapSearch
    {
        public static VSitemap Get(Guid id)
        {
            using (var db = new ReportDbContext())
                return db.VSitemaps.FirstOrDefault(x => x.PageIdentifier == id);
        }

        public static VSitemap[] Get(SitemapFilter filter)
        {
            using (var db = new ReportDbContext())
                return CreateQuery(db, filter).ToArray();
        }

        public static int Count(SitemapFilter filter)
        {
            using (var db = new ReportDbContext())
                return CreateQuery(db, filter).Count();
        }

        private static IQueryable<VSitemap> CreateQuery(ReportDbContext db, SitemapFilter filter)
        {
            var query = db.VSitemaps.AsNoTracking().AsQueryable();

            if (filter.FolderIdentifier.HasValue)
                query = query.Where(y => y.FolderIdentifier == filter.FolderIdentifier);

            if (filter.PageIdentifier.HasValue)
                query = query.Where(y => y.PageIdentifier == filter.PageIdentifier);

            if (filter.PageType.HasValue())
                query = query.Where(y => y.PageType == filter.PageType);

            if (filter.SiteIdentifier.HasValue)
                query = query.Where(y => y.SiteIdentifier == filter.SiteIdentifier);

            query = query.Where(y => y.OrganizationIdentifier == filter.OrganizationIdentifier);

            return query;
        }
    }
}