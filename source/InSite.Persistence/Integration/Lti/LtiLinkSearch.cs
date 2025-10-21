using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class LtiLinkSearch
    {
        private class LtiLinkReadHelper : ReadHelper<TLtiLink>
        {
            public static readonly LtiLinkReadHelper Instance = new LtiLinkReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TLtiLink>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.LtiLinks.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TLtiLink Select(Guid identifier, params Expression<Func<TLtiLink, object>>[] includes) =>
            LtiLinkReadHelper.Instance.SelectFirst(x => x.LinkIdentifier == identifier, includes);

        public static TLtiLink Select(Guid organization, int asset)
        {
            using (var db = new InternalDbContext())
            {
                return db.LtiLinks.FirstOrDefault(x => x.OrganizationIdentifier == organization && x.AssetNumber == asset);
            }
        }

        public static List<TLtiLink> SelectAll(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.LtiLinks
                    .Where(x => x.OrganizationIdentifier == organization)
                    .ToList();
            }
        }

        public static int CountByLtiLinkFilter(LtiLinkFilter filter)
        {
            using (var db = new InternalDbContext())
                return CountByLtiLinkFilter(db, filter);
        }

        private static int CountByLtiLinkFilter(InternalDbContext context, LtiLinkFilter filter) =>
            CreateQueryByLtiLinkFilter(filter, context).Count();

        public static SearchResultList SelectSearchResults(LtiLinkFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByLtiLinkFilter(filter, db)
                    .Select(x => new
                    {
                        x.LinkIdentifier,
                        x.AssetNumber,
                        Publisher = x.ToolProviderName,
                        Title = x.ResourceTitle,
                        Location = x.ResourceName,
                        Subtype = x.ToolProviderType,
                        Code = x.ResourceCode
                    })
                    .OrderBy(x => x.Title)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        private static IQueryable<TLtiLink> CreateQueryByLtiLinkFilter(LtiLinkFilter filter, InternalDbContext db) =>
            FilterQueryByLtiLinkFilter(filter, db.LtiLinks.AsQueryable(), db);

        private static IQueryable<TLtiLink> FilterQueryByLtiLinkFilter(LtiLinkFilter filter, IQueryable<TLtiLink> query, InternalDbContext db)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.Publisher))
                query = query.Where(x => x.ToolProviderName == filter.Publisher);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(x => x.ResourceTitle.Contains(filter.Title));

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(x => x.ResourceName.Contains(filter.Location));

            if (!string.IsNullOrEmpty(filter.Subtype))
                query = query.Where(x => x.ToolProviderType == filter.Subtype);

            if (!string.IsNullOrEmpty(filter.Code))
                query = query.Where(x => x.ResourceCode == filter.Code);

            return query;
        }
    }
}
