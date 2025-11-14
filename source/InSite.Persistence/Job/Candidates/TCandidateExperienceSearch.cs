using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class TCandidateExperienceSearch
    {
        private class CandidateExperienceReadHelper : ReadHelper<TCandidateExperience>
        {
            public static readonly CandidateExperienceReadHelper Instance = new CandidateExperienceReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TCandidateExperience>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TCandidateExperiences.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TCandidateExperience Select(Guid candidateExperienceId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCandidateExperiences.First(x => x.ExperienceIdentifier == candidateExperienceId);
            }
        }

        public static IList<TCandidateExperience> SelectByContact(Guid contactId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCandidateExperiences.Where(x => x.UserIdentifier == contactId).ToList();
            }
        }

        public static IReadOnlyList<TCandidateExperience> Select(Expression<Func<TCandidateExperience, bool>> filter, string sortExpression, params Expression<Func<TCandidateExperience, object>>[] includes) =>
            CandidateExperienceReadHelper.Instance.Select(filter, sortExpression, includes);

        public static TCandidateExperience SelectFirst(Expression<Func<TCandidateExperience, bool>> filter, params Expression<Func<TCandidateExperience, object>>[] includes) =>
            CandidateExperienceReadHelper.Instance.SelectFirst(filter, includes);

        public static int Count(Expression<Func<TCandidateExperience, bool>> filter) =>
            CandidateExperienceReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<TCandidateExperience, bool>> filter) =>
            CandidateExperienceReadHelper.Instance.Exists(filter);

    }
}
