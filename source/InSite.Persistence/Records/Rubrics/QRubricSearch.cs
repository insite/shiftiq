using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;
using InSite.Persistence.Foundation;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class QRubricSearch : IRubricSearch
    {
        #region Other

        private InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        #endregion

        #region QRubric

        public QRubric GetRubric(Guid rubricId)
        {
            using (var db = CreateContext())
                return db.QRubrics.Where(x => x.RubricIdentifier == rubricId).FirstOrDefault();
        }

        public QRubric[] GetRubrics(IEnumerable<Guid> rubricIds)
        {
            using (var db = CreateContext())
                return db.QRubrics.Where(x => rubricIds.Contains(x.RubricIdentifier)).ToArray();
        }

        public RubricSearchItem[] GetRubrics(QRubricFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = !string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(filter.OrderBy)
                    : query.OrderByDescending(x => x.Created);

                return query
                    .ApplyPaging(filter.Paging)
                    .Select(x => new RubricSearchItem
                    {
                        RubricIdentifier = x.RubricIdentifier,
                        RubricTitle = x.RubricTitle,
                        RubricPoints = x.RubricPoints,
                        CriteriaCount = x.RubricCriteria.Count(),
                        Created = x.Created
                    })
                    .ToArray();
            }
        }

        public int CountRubrics(QRubricFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public QRubric GetQuestionRubric(Guid questionId)
        {
            using (var db = CreateContext())
                return db.BankQuestions
                    .Where(x => x.QuestionIdentifier == questionId)
                    .Select(x => x.Rubric)
                    .FirstOrDefault();
        }

        public Dictionary<Guid, QRubric> GetQuestionRubrics(IEnumerable<Guid> questionIds)
        {
            using (var db = CreateContext())
                return db.BankQuestions
                    .Where(x => questionIds.Contains(x.QuestionIdentifier) && x.RubricIdentifier != null)
                    .Select(x => new { x.QuestionIdentifier, x.Rubric })
                    .AsEnumerable()
                    .Where(x => x.Rubric != null)
                    .ToDictionary(x => x.QuestionIdentifier, x => x.Rubric);
        }

        public bool RubricHasAttempts(Guid rubricId)
        {
            using (var db = CreateContext())
                return db.BankQuestions
                    .Where(x => x.RubricIdentifier == rubricId)
                    .Join(db.QAttemptQuestions,
                        a => a.QuestionIdentifier,
                        b => b.QuestionIdentifier,
                        (a, b) => a)
                    .Any();
        }

        private static IQueryable<QRubric> CreateQuery(QRubricFilter filter, InternalDbContext context)
        {
            var query = context.QRubrics.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.RubricTitle))
                query = query.Where(x => x.RubricTitle.Contains(filter.RubricTitle));

            if (filter.CreatedSince.HasValue)
                query = query.Where(x => x.Created >= filter.CreatedSince.Value);

            if (filter.CreatedBefore.HasValue)
                query = query.Where(x => x.Created < filter.CreatedBefore.Value);

            return query;
        }

        #endregion

        #region QRubricCriterion

        public QRubricCriterion GetCriterion(Guid criterionId)
        {
            using (var db = CreateContext())
                return db.QRubricCriteria.Where(x => x.RubricCriterionIdentifier == criterionId).FirstOrDefault();
        }

        public QRubricCriterion[] GetCriteria(Guid rubricId, params Expression<Func<QRubricCriterion, object>>[] includes)
        {
            using (var db = CreateContext())
                return db.QRubricCriteria.ApplyIncludes(includes).Where(x => x.RubricIdentifier == rubricId).ToArray();
        }

        public int CountCriteria(Guid rubricId)
        {
            using (var db = CreateContext())
                return db.QRubricCriteria.Where(x => x.RubricIdentifier == rubricId).Count();
        }

        #endregion

        #region QRubricRating

        public QRubricRating GetRating(Guid ratingId)
        {
            using (var db = CreateContext())
                return db.QRubricRatings.Where(x => x.RubricRatingIdentifier == ratingId).FirstOrDefault();
        }

        public QRubricRating[] GetRatings(Guid[] ratingIds, params Expression<Func<QRubricRating, object>>[] includes)
        {
            using (var db = CreateContext())
                return db.QRubricRatings.ApplyIncludes(includes).Where(x => ratingIds.Contains(x.RubricRatingIdentifier)).ToArray();
        }

        #endregion
    }
}
