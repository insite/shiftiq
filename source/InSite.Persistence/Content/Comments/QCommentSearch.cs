using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contents.Read;

namespace InSite.Persistence
{
    public static class QCommentSearch
    {
        #region Classes

        private class QCommentReadHelper : ReadHelper<QComment>
        {
            public static readonly QCommentReadHelper Instance = new QCommentReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QComment>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.QComments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region SELECT

        public static QComment SelectFirst(
            Guid id,
            params Expression<Func<QComment, object>>[] includes) =>
            QCommentReadHelper.Instance.SelectFirst(x => x.CommentIdentifier == id, includes);

        public static QComment SelectFirst(
            Expression<Func<QComment, bool>> filter,
            params Expression<Func<QComment, object>>[] includes) =>
            QCommentReadHelper.Instance.SelectFirst(filter, includes);

        public static T[] Bind<T>(
            Expression<Func<QComment, T>> binder,
            Expression<Func<QComment, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            QCommentReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Guid id,
            Expression<Func<QComment, T>> binder,
            string modelSort = null,
            string entitySort = null) =>
            QCommentReadHelper.Instance.BindFirst(binder, x => x.CommentIdentifier == id, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<QComment, T>> binder,
            Expression<Func<QComment, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            QCommentReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<QComment, bool>> filter) =>
            QCommentReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<QComment, bool>> filter) =>
            QCommentReadHelper.Instance.Exists(filter);

        public static List<VComment> Search(Guid organization, Guid container)
        {
            using (var db = new InternalDbContext())
            {
                return db.VComments
                    .Where(x => x.OrganizationIdentifier == organization && x.ContainerIdentifier == container)
                    .ToList();
            }
        }

        public static List<QComment> GetPersonComments(Guid userId, Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                return db.QComments
                    .Where(x => x.TopicUserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .ToList();
            }
        }

        #endregion
    }
}
