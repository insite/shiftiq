using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Quizzes.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class TQuizSearch : IQuizSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public int Count(TQuizFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                return query.Count();
            }
        }

        public List<TQuiz> Select(TQuizFilter filter, params Expression<Func<TQuiz, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderBy(nameof(TQuiz.QuizName));

                return query.ApplyPaging(filter).ToList();
            }
        }

        public TQuiz Select(Guid quizId, params Expression<Func<TQuiz, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.TQuizzes.AsNoTracking()
                    .Where(x => x.QuizIdentifier == quizId)
                    .FirstOrDefault();
            }
        }

        private IQueryable<TQuiz> CreateQuery(TQuizFilter filter, InternalDbContext db)
        {
            var query = db.TQuizzes.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.QuizType.IsNotEmpty())
                query = query.Where(x => x.QuizType == filter.QuizType);

            if (filter.QuizNameContains.IsNotEmpty())
                query = query.Where(x => x.QuizName.Contains(filter.QuizNameContains));

            if (filter.QuizDataContains.IsNotEmpty())
                query = query.Where(x => x.QuizData.Contains(filter.QuizDataContains));

            if (filter.TimeLimitFrom.HasValue)
                query = query.Where(x => x.TimeLimit >= filter.TimeLimitFrom.Value);

            if (filter.TimeLimitThru.HasValue)
                query = query.Where(x => x.TimeLimit <= filter.TimeLimitThru.Value);

            if (filter.AttemptLimitFrom.HasValue)
                query = query.Where(x => x.AttemptLimit >= filter.AttemptLimitFrom);

            if (filter.AttemptLimitThru.HasValue)
                query = query.Where(x => x.AttemptLimit <= filter.AttemptLimitThru);

            return query;
        }
    }
}
