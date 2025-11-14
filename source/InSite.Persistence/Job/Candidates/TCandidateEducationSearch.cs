using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class TCandidateEducationSearch
    {
        private class CandidateEducationReadHelper : ReadHelper<TCandidateEducation>
        {
            public static readonly CandidateEducationReadHelper Instance = new CandidateEducationReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TCandidateEducation>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TCandidateEducations.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TCandidateEducation Select(Guid candidateEducationId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCandidateEducations.First(x => x.EducationIdentifier == candidateEducationId);
            }
        }

        public static IList<TCandidateEducation> SelectByContact(Guid contactId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCandidateEducations.Where(x => x.UserIdentifier == contactId).ToList();
            }
        }

        public static IReadOnlyList<TCandidateEducation> Select(Expression<Func<TCandidateEducation, bool>> filter, string sortExpression, params Expression<Func<TCandidateEducation, object>>[] includes) =>
            CandidateEducationReadHelper.Instance.Select(filter, sortExpression, includes);

        public static TCandidateEducation SelectFirst(Expression<Func<TCandidateEducation, bool>> filter, params Expression<Func<TCandidateEducation, object>>[] includes) =>
            CandidateEducationReadHelper.Instance.SelectFirst(filter, includes);

        public static int Count(Expression<Func<TCandidateEducation, bool>> filter) =>
            CandidateEducationReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<TCandidateEducation, bool>> filter) =>
            CandidateEducationReadHelper.Instance.Exists(filter);
    }
}
