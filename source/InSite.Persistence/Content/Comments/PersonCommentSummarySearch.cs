using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contents.Read;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class PersonCommentSummarySearch
    {
        #region Helper methods

        private static IQueryable<VComment> Filter(IQueryable<VComment> query, PersonCommentSummaryFilter filter)
        {
            if (filter.UtcPostedSince.HasValue)
                query = query.Where(x => x.CommentPosted >= filter.UtcPostedSince.Value);

            if (filter.UtcPostedBefore.HasValue)
                query = query.Where(x => x.CommentPosted < filter.UtcPostedBefore.Value);

            if (!string.IsNullOrEmpty(filter.AuthorName))
                query = query.Where(x => x.AuthorUserName.Contains(filter.AuthorName));

            if (!string.IsNullOrEmpty(filter.AuthorEmail))
                query = query.Where(x => x.AuthorEmail.Contains(filter.AuthorEmail));

            if (!string.IsNullOrEmpty(filter.Keyword))
                query = query.Where(x => x.UploadName.Contains(filter.Keyword) || x.CommentText.Contains(filter.Keyword));

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.TopicUserIdentifier == filter.UserIdentifier);

            if (filter.DescriptionInclusion.HasValue)
            {
                if (filter.DescriptionInclusion == InclusionType.Only)
                    query = query.Where(x => x.CommentText != null);
                else if (filter.ReminderInclusion == InclusionType.Exclude)
                    query = query.Where(x => x.CommentText == null);
            }

            return query;
        }

        #endregion

        #region Classes

        private class PersonCommentSummaryReadHelper : ReadHelper<VComment>
        {
            public static readonly PersonCommentSummaryReadHelper Instance = new PersonCommentSummaryReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VComment>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.VComments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Select (entity)

        public static List<VComment> SelectForCommentRepeater(Guid user, Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.VComments
                    .Where(x => x.TopicUserIdentifier == user && x.OrganizationIdentifier == organization)
                    .OrderByDescending(x => x.CommentPosted)
                    .ToList();
            }
        }

        public static IReadOnlyList<VComment> Select(
            Expression<Func<VComment, bool>> filter,
            params Expression<Func<VComment, object>>[] includes)
        {
            return PersonCommentSummaryReadHelper.Instance.Select(filter, includes);
        }

        public static VComment SelectFirst(
            Expression<Func<VComment, bool>> filter,
            params Expression<Func<VComment, object>>[] includes)
        {
            return PersonCommentSummaryReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<VComment> Select(
            Expression<Func<VComment, bool>> filter,
            string sortExpression,
            params Expression<Func<VComment, object>>[] includes)
        {
            return PersonCommentSummaryReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static VComment SelectFirst(
            Expression<Func<VComment, bool>> filter,
            string sortExpression,
            Expression<Func<VComment, object>>[] includes)
        {
            return PersonCommentSummaryReadHelper.Instance.SelectFirst(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<VComment, T>> binder,
            Expression<Func<VComment, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return PersonCommentSummaryReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<VComment, T>> binder,
            Expression<Func<VComment, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return PersonCommentSummaryReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<VComment, bool>> filter)
        {
            return PersonCommentSummaryReadHelper.Instance.Count(filter);
        }

        public static int Count(PersonCommentSummaryFilter filter)
        {
            return PersonCommentSummaryReadHelper.Instance.Count(query => Filter(query, filter));
        }

        public static bool Exists(Expression<Func<VComment, bool>> filter)
        {
            return PersonCommentSummaryReadHelper.Instance.Exists(filter);
        }

        #endregion
    }
}