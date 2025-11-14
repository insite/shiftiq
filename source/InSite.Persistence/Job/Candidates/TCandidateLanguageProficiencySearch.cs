using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class TCandidateLanguageProficiencySearch
    {
        private class CandidateLanguageProficiencyReadHelper : ReadHelper<TCandidateLanguageProficiency>
        {
            public static readonly CandidateLanguageProficiencyReadHelper Instance = new CandidateLanguageProficiencyReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TCandidateLanguageProficiency>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TCandidateLanguageProficiencies.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TCandidateLanguageProficiency SelectFirstLanguage(Guid userId, Guid organizationId)
        {
            using (var context = new InternalDbContext())
            {
                return context.TCandidateLanguageProficiencies
                    .Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .OrderBy(x => x.Sequence)
                    .FirstOrDefault();
            }
        }

        public static IReadOnlyList<TCandidateLanguageProficiency> SelectByUser(
            Guid userId,
            Guid organizationId,
            params Expression<Func<TCandidateLanguageProficiency, object>>[] includes
            )
        {
            return CandidateLanguageProficiencyReadHelper.Instance
                .Select(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId, includes)
                .OrderBy(x => x.Sequence)
                .ToList();
        }

        public static bool Exists(Guid userId, Guid organizationId)
        {
            return CandidateLanguageProficiencyReadHelper.Instance
                .Exists(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId);
        }

        public static TCandidateLanguageProficiency SelectFirst(Expression<Func<TCandidateLanguageProficiency, bool>> filter, params Expression<Func<TCandidateLanguageProficiency, object>>[] includes) =>
            CandidateLanguageProficiencyReadHelper.Instance.SelectFirst(filter, includes);

        public static int Count(Expression<Func<TCandidateLanguageProficiency, bool>> filter) =>
            CandidateLanguageProficiencyReadHelper.Instance.Count(filter);
    }
}
