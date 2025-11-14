using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TOpportunitySearch
    {
        private static readonly string[] KeywordConjunctions = new string[] { " this ", " is ", " no ", " it ", " and ", " a ", " to ", " for ", " the ", " at ", " in ", " my " };
        private static readonly char[] KeywordGarbageChars = new char[] { ';', ':', '\'', '"', '-', ',', '&', '/', '\\', '(', ')', '[', ']' };

        private class TOpportunityReadHelper : ReadHelper<TOpportunity>
        {
            public static readonly TOpportunityReadHelper Instance = new TOpportunityReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TOpportunity>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TOpportunities.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TOpportunity Select(Guid key, params Expression<Func<TOpportunity, object>>[] includes) =>
            TOpportunityReadHelper.Instance.SelectFirst(x => x.OpportunityIdentifier == key, includes);

        public static int Count(Expression<Func<TOpportunity, bool>> filter) =>
            TOpportunityReadHelper.Instance.Count(filter);

        public static TOpportunity Select(Guid opportunityId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TOpportunities.FirstOrDefault(x => x.OpportunityIdentifier == opportunityId);
            }
        }

        public static TOpportunity[] Select(IEnumerable<Guid> ids)
        {
            using (var db = new InternalDbContext())
            {
                return db.TOpportunities.Where(x => ids.Contains(x.OpportunityIdentifier)).ToArray();
            }
        }

        public static IList<TOpportunity> SelectByEmployer(Guid contactId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TOpportunities.Where(x => x.EmployerUserIdentifier == contactId).ToList();
            }
        }

        public static TOpportunity[] SelectByJobFilter(TOpportunityFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        public static SearchResultList SelectAdminSearchResults(TOpportunityFilter filter)
        {
            var sortExpression = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "WhenCreated desc";

            using (var db = new InternalDbContext())
            {
                var list = CreateQuery(filter, db)
                    .OrderBy(sortExpression)
                    .Select(x => new
                    {
                        OpportunityIdentifier = x.OpportunityIdentifier,
                        EmployerGroupIdentifier = x.EmployerGroupIdentifier,
                        EmployerGroupName = x.EmployerGroupName,
                        EmployerGroupDescription = x.EmployerGroupDescription,
                        EmployerUserIdentifier = x.EmployerUserIdentifier,
                        EmploymentType = x.EmploymentType,
                        JobTitle = x.JobTitle,
                        JobDescription = x.JobDescription,
                        ApplicationEmail = x.ApplicationEmail,
                        ApplicationWebSiteUrl = x.ApplicationWebSiteUrl,
                        ApplicationRequiresResume = x.ApplicationRequiresResume,
                        ApplicationRequiresLetter = x.ApplicationRequiresLetter,
                        LocationName = x.LocationName,
                        LocationType = x.LocationType,
                        WhenPublished = x.WhenPublished,
                        WhenCreated = x.WhenCreated,
                        WhenModified = x.WhenModified,
                        OccupationAreaIdentifier = x.OccupationStandardIdentifier,
                        OccupationAreaName = x.OccupationStandard.ContentTitle
                    })
                    .ApplyPaging(filter)
                    .ToList();


                return list.ToSearchResult();
            }
        }

        public static List<TOpportunity> SelectSearchResults(TOpportunityFilter filter)
        {
            var sortExpression = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "WhenCreated desc";

            using (var db = new InternalDbContext())
            {
                var list = CreateQuery(filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToList();

                return list;
            }
        }

        public static int Count(TOpportunityFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db).Count();
            }
        }

        private static IQueryable<TOpportunity> CreateQuery(TOpportunityFilter filter, InternalDbContext db)
        {
            var query = db.TOpportunities.AsQueryable();

            if (filter == null)
                return query;

            if (filter.DepartmentGroupIdentifier.HasValue)
            {
                var children = db.QGroupConnections
                    .Where(x => x.ParentGroupIdentifier == filter.DepartmentGroupIdentifier)
                    .Select(x => x.ChildGroupIdentifier)
                    .ToArray();

                query = query.Where(x => children.Any(y => y == x.DepartmentGroupIdentifier));
            }

            if (filter.OccupationStandardIdentifier.HasValue)
                query = query.Where(x => x.OccupationStandardIdentifier == filter.OccupationStandardIdentifier.Value);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.EmployerGroupIdentifier.HasValue)
                query = query.Where(x => x.EmployerGroupIdentifier == filter.EmployerGroupIdentifier.Value);

            if (filter.TemplateIdentifier.HasValue)
                query = query.Where(x => x.DepartmentGroupIdentifier == filter.TemplateIdentifier.Value);

            if (filter.JobTitle.HasValue())
                query = query.Where(x => x.JobTitle.Contains(filter.JobTitle));

            if (filter.CompanyName.HasValue())
                query = query.Where(x => x.EmployerGroupName.Contains(filter.CompanyName));

            if (filter.JobLocation.HasValue())
                query = query.Where(x => x.LocationName.Contains(filter.JobLocation));

            if (filter.EmploymentType.HasValue())
            {
                if (filter.EmploymentType.Equals("FullTime"))
                    query = query.Where(x => x.EmploymentType.Equals("Full time"));
                else if (filter.EmploymentType.Equals("PartTime"))
                    query = query.Where(x => x.EmploymentType.Equals("Part time"));
                else
                    query = query.Where(x => x.EmploymentType.Contains(filter.EmploymentType));
            }

            if (filter.PositionType.HasValue())
                query = query.Where(x => x.LocationType.Contains(filter.PositionType));

            if (filter.JobType.HasValue())
                query = query.Where(x => x.EmploymentType.Contains(filter.JobType));

            if (filter.PostedSince.HasValue)
                query = query.Where(x => x.ApplicationOpen >= filter.PostedSince.Value);

            if (filter.PostedBefore.HasValue)
                query = query.Where(x => x.ApplicationOpen < filter.PostedBefore.Value);

            if (filter.PublishedSince.HasValue)
                query = query.Where(x => x.WhenPublished >= filter.PublishedSince.Value);

            if (filter.PublishedBefore.HasValue)
                query = query.Where(x => x.WhenPublished < filter.PublishedBefore.Value);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.EmployerUserIdentifier == filter.UserIdentifier.Value);

            var now = DateTimeOffset.UtcNow;

            if (filter.IsPublished.HasValue)
            {
                query = filter.IsPublished.Value
                    ? query.Where(x => x.WhenPublished.HasValue && x.WhenPublished <= now)
                    : query.Where(x => !x.WhenPublished.HasValue || x.WhenPublished > now);
            }

            if (filter.IsClosed.HasValue)
            {
                query = filter.IsClosed.Value
                    ? query.Where(x => x.WhenClosed.HasValue && x.WhenClosed <= now)
                    : query.Where(x => !x.WhenClosed.HasValue || x.WhenClosed > now);
            }

            if (filter.IsArchived.HasValue)
            {
                query = filter.IsArchived.Value
                    ? query.Where(x => x.WhenArchived.HasValue && x.WhenArchived <= now)
                    : query.Where(x => !x.WhenArchived.HasValue || x.WhenArchived > now);
            }

            if (!string.IsNullOrEmpty(filter.Keywords))
            {
                var builder = new StringBuilder(filter.Keywords.Trim());

                foreach (var garbageChar in KeywordGarbageChars) builder.Replace(garbageChar, ' ');
                foreach (var conjunction in KeywordConjunctions) builder.Replace(conjunction, " ");
                builder.Replace("  ", " ");

                var keywords = builder.ToString().Split(new char[] { ' ', ',' });
                foreach (var keyword in keywords)
                {
                    query = query.Where(x =>
                        x.JobTitle.Contains(keyword)
                        || x.LocationName.Contains(keyword)
                        || x.EmploymentType.Contains(keyword)
                        || x.EmployerGroupName.Contains(keyword)
                        || x.JobLevel.Contains(keyword)
                    );
                }
            }

            return query;
        }
    }
}
