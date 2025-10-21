using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class TCandidateUploadSearch
    {
        private class UploadReadHelper : ReadHelper<TCandidateUpload>
        {
            public static readonly UploadReadHelper Instance = new UploadReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TCandidateUpload>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TCandidateUploads.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static IList<TCandidateUpload> SelectByContact(Guid contactId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCandidateUploads.Where(x => x.CandidateUserIdentifier == contactId).ToList();
            }
        }

        public static IReadOnlyList<TCandidateUpload> Select(
            Expression<Func<TCandidateUpload, bool>> filter,
            string sortExpression,
            params Expression<Func<TCandidateUpload, object>>[] includes
            ) =>
            UploadReadHelper.Instance.Select(filter, sortExpression, includes);

        public static TCandidateUpload SelectFirst(
            Expression<Func<TCandidateUpload, bool>> filter,
            params Expression<Func<TCandidateUpload, object>>[] includes
            ) =>
            UploadReadHelper.Instance.SelectFirst(filter, includes);

        public static int Count(Expression<Func<TCandidateUpload, bool>> filter) =>
            UploadReadHelper.Instance.Count(filter);
    }
}
