using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Sites.Read;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class SiteSearch : ISiteSearch
    {

        #region Classes

        private class ReadHelper : ReadHelper<QSite>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QSite>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.QSites.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static class QSiteFilterHelper
        {
            public static IQueryable<QSite> ApplyFilter(IQueryable<QSite> query, QSiteFilter filter)
            {
                if (filter.OrganizationIdentifier.HasValue)
                    query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

                if (!string.IsNullOrEmpty(filter.Title))
                    query = query.Where(x => x.SiteTitle.Contains(filter.Title));

                if (!string.IsNullOrEmpty(filter.Domain))
                    query = query.Where(x => x.SiteDomain.Contains(filter.Domain));

                if (!string.IsNullOrEmpty(filter.Keyword))
                    query = query.Where(x => x.SiteTitle.Contains(filter.Keyword) || x.SiteDomain.Contains(filter.Keyword));

                if (filter.LastModifiedSince.HasValue)
                    query = query.Where(x => filter.LastModifiedSince.Value <= x.LastChangeTime);

                if (filter.LastModifiedBefore.HasValue)
                    query = query.Where(x => x.LastChangeTime < filter.LastModifiedBefore.Value);

                return query;
            }
        }

        #endregion

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public QSite Select(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.QSites
                    .AsNoTracking()
                    .FirstOrDefault(x => x.SiteIdentifier == id);
            }
        }

        public QSite Select(string title, Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.QSites
                    .AsNoTracking()
                    .FirstOrDefault(x => x.SiteTitle == title
                        && x.OrganizationIdentifier == organizationId);
            }
        }

        public int Count(Expression<Func<QSite, bool>> filter)
        {
            using (var db = new InternalDbContext())
                return ReadHelper.Instance.Count(filter);
        }

        public CountInfo[] SelectCount(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var site = new CountInfo
                {
                    Name = "Site",
                    Count = db.QSites.Where(x => x.OrganizationIdentifier == organizationId).Count()
                };
                var pages = db.QPages.AsQueryable().Where(x => x.OrganizationIdentifier == organizationId).GroupBy(x => x.PageType).Select(x => new CountInfo
                {
                    Name = x.Key,
                    Count = x.Count()
                });

                return pages.AsEnumerable().Prepend(site).ToArray();
            }
        }

        #region Select (entity)

        public QSite Select(Guid id, params Expression<Func<QSite, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(x => x.SiteIdentifier == id, includes);

        public static QSite SelectFirst(Expression<Func<QSite, bool>> filter, params Expression<Func<QSite, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(filter, includes);

        public T[] Bind<T>(
            Expression<Func<QSite, T>> binder,
            Expression<Func<QSite, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public T BindFirst<T>(
            Expression<Func<QSite, T>> binder,
            Expression<Func<QSite, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        #endregion


        #region Select (filter)

        public int Count(QSiteFilter filter)
        {
            using (var db = new InternalDbContext())
                return QSiteFilterHelper.ApplyFilter(db.QSites.AsQueryable().AsNoTracking(), filter).Count();
        }

        public T[] Bind<T>(
            Expression<Func<QSite, T>> binder,
            QSiteFilter filter) =>
            ReadHelper.Instance.Bind(
                (IQueryable<QSite> query) => query.Select(binder),
                (IQueryable<QSite> query) => QSiteFilterHelper.ApplyFilter(query, filter),
                filter.Paging, filter.OrderBy, null);

        #endregion

        public RecentInfo[] SelectRecent(Guid organizationId, int take)
        {
            var sort = "Modified DESC";

            using (var db = new InternalDbContext())
            {
                var sites = db.QSites.AsQueryable().Where(x => x.OrganizationIdentifier == organizationId).Select(x => new RecentInfo
                {
                    Identifier = x.SiteIdentifier,
                    Type = "Site",
                    Title = x.SiteTitle,
                    Name = x.SiteDomain,
                    ModifiedBy = Guid.Empty,
                    Modified = x.LastChangeTime.Value
                });
                var resources = db.QPages.AsQueryable().Where(x => x.OrganizationIdentifier == organizationId).Select(x => new RecentInfo
                {
                    Identifier = x.PageIdentifier,
                    Type = x.PageType,
                    Title = x.Title,
                    Name = x.PageSlug,
                    ModifiedBy = Guid.Empty,
                    Modified = x.LastChangeTime.Value
                });

                return sites.Union(resources)
                    .OrderBy(sort)
                    .Take(take)
                    .ToArray();
            }
        }
    }
}