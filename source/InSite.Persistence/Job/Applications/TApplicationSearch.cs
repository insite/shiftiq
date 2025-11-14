using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TApplicationSearch
    {
        private class ApplicationReadHelper : ReadHelper<TApplication>
        {
            public static readonly ApplicationReadHelper Instance = new ApplicationReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TApplication>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TApplications.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static IList<TApplication> SelectJobApplicationByCandidate(Guid candidateContactId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TApplications.Where(x => x.CandidateUserIdentifier == candidateContactId).ToList();
            }
        }

        public static TApplication SelectJobApplication(Guid candidateContactId, Guid opportunityId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TApplications.FirstOrDefault(x => x.CandidateUserIdentifier == candidateContactId
                    && x.OpportunityIdentifier == opportunityId);
            }
        }

        public static IList<TApplication> SelectJobApplicationByJob(Guid opportunityId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TApplications.Where(x => x.OpportunityIdentifier == opportunityId).ToList();
            }
        }

        public static TApplication SelectJobApplication(Guid key, params Expression<Func<TApplication, object>>[] includes) =>
            ApplicationReadHelper.Instance.SelectFirst(x => x.ApplicationIdentifier == key, includes);

        public static int CountJobApplication(Expression<Func<TApplication, bool>> filter) =>
            ApplicationReadHelper.Instance.Count(filter);

        public static int Count(TApplicationFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        public static SearchResultList SelectSearchResults(TApplicationFilter filter)
        {
            var sortExpression = "Created DESC";

            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .Select(x => new
                    {
                        ApplicationIdentifier = x.ApplicationIdentifier,
                        OpportunityIdentifier = x.OpportunityIdentifier,
                        EmployerName = x.Opportunity.EmployerGroupName,
                        JobPosition = x.Opportunity.JobTitle,
                        CandidateUserIdentifier = x.CandidateUserIdentifier,
                        CandidateFirstName = x.CandidateUser.FirstName,
                        CandidateLastName = x.CandidateUser.LastName,
                        CandidateLetter = x.CandidateLetter,
                        CandidateResume = x.CandidateResume,
                        Created = x.WhenCreated,
                        Updated = x.WhenModified
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        private static IQueryable<TApplication> CreateQuery(TApplicationFilter filter, InternalDbContext db)
        {
            var query = db.TApplications.AsQueryable();

            if (filter == null)
                return query;

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Opportunity.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.EmployerGroupIdentifier.HasValue)
                query = query.Where(x => x.Opportunity.EmployerGroupIdentifier == filter.EmployerGroupIdentifier);

            if (filter.OpportunityIdentifier.HasValue)
                query = query.Where(x => x.OpportunityIdentifier == filter.OpportunityIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.CandidateUserIdentifier == filter.UserIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.EmployerName))
                query = query.Where(x => x.Opportunity.EmployerGroupName.Contains(filter.EmployerName));

            if (!string.IsNullOrEmpty(filter.JobTitle))
                query = query.Where(x => x.Opportunity.JobTitle.Contains(filter.JobTitle));

            if (filter.DateUpdatedSince.HasValue)
                query = query.Where(x => x.WhenModified >= filter.DateUpdatedSince.Value);

            if (filter.DateUpdatedBefore.HasValue)
                query = query.Where(x => x.WhenModified < filter.DateUpdatedBefore.Value);

            return query;
        }
    }
}
