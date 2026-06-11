using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.QuizAttempts.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class TQuizAttemptSearch : IQuizAttemptSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public int Count(TQuizAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                return query.Count();
            }
        }

        public List<TQuizAttempt> Select(TQuizAttemptFilter filter, params Expression<Func<TQuizAttempt, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderBy(nameof(TQuizAttempt.AttemptStarted) + " DESC");

                return query.ApplyPaging(filter).ToList();
            }
        }

        public TQuizAttempt Select(Guid attemptId, params Expression<Func<TQuizAttempt, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.TQuizAttempts.AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.AttemptIdentifier == attemptId)
                    .FirstOrDefault();
            }
        }

        public TQuizAttempt SelectLatest(TQuizAttemptFilter filter, params Expression<Func<TQuizAttempt, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .ApplyIncludes(includes)
                    .OrderByDescending(x => x.AttemptCreated)
                    .FirstOrDefault();
            }
        }

        private IQueryable<TQuizAttempt> CreateQuery(TQuizAttemptFilter filter, InternalDbContext db)
        {
            var query = db.TQuizAttempts.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.QuizIdentifier.HasValue)
                query = query.Where(x => x.QuizIdentifier == filter.QuizIdentifier.Value);

            if (filter.LearnerIdentifier.HasValue)
                query = query.Where(x => x.LearnerIdentifier == filter.LearnerIdentifier.Value);

            if (filter.QuizType.IsNotEmpty())
                query = query.Where(x => x.Quiz.QuizType == filter.QuizType);

            if (filter.QuizNameContains.IsNotEmpty())
                query = query.Where(x => x.Quiz.QuizName.Contains(filter.QuizNameContains));

            if (filter.LearnerNameContains.IsNotEmpty())
                query = query.Where(x => x.LearnerUser.UserFullName.Contains(filter.LearnerNameContains));

            if (filter.LearnerEmailContains.IsNotEmpty())
                query = query.Where(x => x.LearnerUser.UserEmail.Contains(filter.LearnerEmailContains));

            if (filter.IsCompleted.HasValue)
            {
                if (filter.IsCompleted.Value)
                    query = query.Where(x => x.AttemptCompleted.HasValue);
                else
                    query = query.Where(x => !x.AttemptCompleted.HasValue);
            }

            if (filter.AttemptStartedSince.HasValue)
                query = query.Where(x => x.AttemptStarted >= filter.AttemptStartedSince);

            if (filter.AttemptStartedBefore.HasValue)
                query = query.Where(x => x.AttemptStarted < filter.AttemptStartedBefore);

            return query;
        }
    }
}
