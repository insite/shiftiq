using System;
using System.Data.Entity;
using System.Linq;

using InSite.Domain.Integration;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class ApiRequestSearch
    {
        public static ApiRequest Select(Guid key)
        {
            using (var db = new InternalDbContext())
            {
                return db.ApiRequests.FirstOrDefault(x => x.RequestIdentifier == key);
            }
        }

        public static ApiRequest SelectByRequestLogKey(Guid key)
        {
            using (var db = new InternalDbContext())
            {
                return db.ApiRequests.FirstOrDefault(x => x.ResponseLogIdentifier == key);
            }
        }

        public static int Count(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.ApiRequests.Count(x => x.OrganizationIdentifier == organization);
            }
        }

        public static SearchResultList SelectByFilter(ApiRequestFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .Include(x => x.User)
                    .Include(x => x.Organization)
                    .OrderByDescending(x => x.RequestStarted)
                    .ApplyPaging(filter)
                    .Select(x => new
                    {
                        x.RequestIdentifier,
                        x.RequestStarted,
                        ContactName = x.User.FullName,
                        OrganizationName = x.Organization.CompanyName,
                        x.RequestUri,
                        x.RequestMethod,
                        x.ResponseStatusNumber,
                        x.ResponseStatusName,
                        x.ResponseTime
                    })
                    .ToSearchResult();
            }
        }

        public static int Count(ApiRequestFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db).Count();
            }
        }

        private static IQueryable<ApiRequest> CreateQuery(ApiRequestFilter filter, InternalDbContext db)
        {
            var query = db.ApiRequests
                .AsNoTracking()
                .AsQueryable();

            if (filter.IsSchemaOnly)
                return query.Where(x => false);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.RequestStartedSince.HasValue)
                query = query.Where(x => x.RequestStarted >= filter.RequestStartedSince.Value);

            if (filter.RequestStartedBefore.HasValue)
                query = query.Where(x => x.RequestStarted < filter.RequestStartedBefore.Value);

            if (filter.RequestUri.IsNotEmpty())
                query = query.Where(x => x.RequestUri.Contains(filter.RequestUri));

            if (filter.RequestIsIncoming.HasValue)
                query = query.Where(x => x.RequestDirection == (filter.RequestIsIncoming.Value ? "In" : "Out"));

            if (filter.RequestData.IsNotEmpty())
                query = query.Where(x => x.ResponseContentData.Contains(filter.RequestData) || x.RequestContentData.Contains(filter.RequestData));

            return query;
        }
    }
}
