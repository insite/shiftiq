using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TCandidateCommentSearch
    {
        private class CandidateCommentReadHelper : ReadHelper<VCandidateComment>
        {
            public static readonly CandidateCommentReadHelper Instance = new CandidateCommentReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VCandidateComment>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.VCandidateComments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static VCandidateComment Select(Guid commentId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCandidateComments.First(x => x.CommentIdentifier == commentId);
            }
        }

        public static IList<VCandidateComment> SelectByAuthor(Guid authorId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCandidateComments.Where(x => x.AuthorUserIdentifier == authorId).ToList();
            }
        }

        public static IList<VCandidateComment> SelectByCandidate(Guid candidateId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCandidateComments.Where(x => x.CandidateUserIdentifier == candidateId).ToList();
            }
        }

        public static int Count(Expression<Func<VCandidateComment, bool>> filter) =>
            CandidateCommentReadHelper.Instance.Count(filter);

        public static int CountByFilter(TCandidateCommentFilter filter)
        {
            using (var db = new InternalDbContext())
                return CountByFilter(db, filter);
        }

        private static int CountByFilter(InternalDbContext context, TCandidateCommentFilter filter) =>
            CreateQueryByFilter(filter, context).Count();

        public static SearchResultList SelectSearchResults(TCandidateCommentFilter filter)
        {
            var sortExpression = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "CommentModified desc";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        CommentIdentifier = x.CommentIdentifier,
                        AuthorUserIdentifier = x.AuthorUserIdentifier,
                        AuthorName = x.Author.FirstName + " " + x.Author.LastName,
                        CandidateUserIdentifier = x.CandidateUserIdentifier,
                        CandidateName = x.Candidate.FirstName + " " + x.Candidate.LastName,
                        CommentIsFlagged = x.CommentIsFlagged,
                        CommentText = x.CommentText,
                        CommentModified = x.CommentModified
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        private static IQueryable<VCandidateComment> CreateQueryByFilter(TCandidateCommentFilter filter, InternalDbContext db) =>
            FilterQueryByFilter(filter, db.VCandidateComments.AsQueryable());

        private static IQueryable<VCandidateComment> FilterQueryByFilter(TCandidateCommentFilter filter, IQueryable<VCandidateComment> query)
        {
            if (filter.AuthorContactId.HasValue)
                query = query.Where(x => x.AuthorUserIdentifier == filter.AuthorContactId);

            if (filter.SubjectContactId.HasValue)
                query = query.Where(x => x.CandidateUserIdentifier == filter.SubjectContactId);

            return query;
        }
    }
}
