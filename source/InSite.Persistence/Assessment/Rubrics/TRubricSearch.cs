using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Persistence.Foundation;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TRubricSearch
    {
        #region Classes

        public class SearchResultsItem
        {
            public Guid RubricIdentifier { get; set; }
            public string RubricTitle { get; set; }
            public decimal RubricPoints { get; set; }
            public int CriteriaCount { get; set; }
            public DateTimeOffset Created { get; set; }
        }

        #endregion

        #region RubricReadHelper

        private class RubricReadHelper : ReadHelper<TRubric>
        {
            public static readonly RubricReadHelper Instance = new RubricReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TRubric>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TRubrics.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static int Count(Expression<Func<TRubric, bool>> filter) =>
            RubricReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<TRubric, bool>> filter) =>
            RubricReadHelper.Instance.Exists(filter);

        public static T[] Bind<T>(
            Expression<Func<TRubric, T>> binder,
            Expression<Func<TRubric, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            RubricReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<TRubric, T>> binder,
            Expression<Func<TRubric, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            RubricReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static T[] Distinct<T>(
            Expression<Func<TRubric, T>> binder,
            Expression<Func<TRubric, bool>> filter,
            string modelSort = null) =>
            RubricReadHelper.Instance.Distinct(binder, filter, modelSort);

        #endregion

        #region TRubricFilter

        public static int Count(TRubricFilter filter)
        {
            using (var context = CreateContext())
                return CreateQuery(filter, context).Count();
        }

        public static List<SearchResultsItem> SelectSearchResults(TRubricFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = !string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(filter.OrderBy)
                    : query.OrderByDescending(x => x.Created);

                return query
                    .ApplyPaging(filter.Paging)
                    .Select(x => new SearchResultsItem
                    {
                        RubricIdentifier = x.RubricIdentifier,
                        RubricTitle = x.RubricTitle,
                        RubricPoints = x.RubricPoints,
                        CriteriaCount = x.Criteria.Count(),
                        Created = x.Created
                    })
                    .ToList();
            }
        }

        private static IQueryable<TRubric> CreateQuery(TRubricFilter filter, InternalDbContext context)
        {
            var query = context.TRubrics.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.RubricTitle))
                query = query.Where(x => x.RubricTitle.Contains(filter.RubricTitle));

            if (filter.CreatedSince.HasValue)
                query = query.Where(x => x.Created >= filter.CreatedSince.Value);

            if (filter.CreatedBefore.HasValue)
                query = query.Where(x => x.Created < filter.CreatedBefore.Value);

            return query;
        }

        #endregion

        #region Select TRubric

        public static Dictionary<Guid, TRubric> SelectRubricsByQuestions(IEnumerable<Guid> questionIdentifiers)
        {
            using (var context = CreateContext())
            {
                return context.TRubricConnections
                    .Where(x => questionIdentifiers.Contains(x.QuestionIdentifier))
                    .Select(x => new { x.QuestionIdentifier, x.Rubric })
                    .ToList()
                    .ToDictionary(x => x.QuestionIdentifier, x => x.Rubric);
            }
        }

        public static TRubric SelectRubricByQuestion(Guid questionIdentifier)
        {
            using (var context = CreateContext())
            {
                return context.TRubricConnections
                    .Where(x => x.QuestionIdentifier == questionIdentifier)
                    .Select(x => x.Rubric)
                    .FirstOrDefault();
            }
        }

        public static List<Guid> SelectQuestionsNotConnectedToRubrics(Guid[] questionIdentifiers)
        {
            using (var context = CreateContext())
            {
                var existingQuestionIdentifiers = context.TRubricConnections
                    .Where(x => questionIdentifiers.Any(q => x.QuestionIdentifier == q))
                    .Select(x => x.QuestionIdentifier)
                    .ToList();

                return questionIdentifiers.Except(existingQuestionIdentifiers).ToList();
            }
        }

        #endregion

        #region Select TRubricCriterion & TRubricRating

        public static List<TRubricCriterion> SelectRubricCriteria(Guid rubricIdentifier, params Expression<Func<TRubricCriterion, object>>[] includes)
        {
            using (var context = CreateContext())
            {
                var query = context.TRubricCriteria
                    .Where(x => x.RubricIdentifier == rubricIdentifier)
                    .ApplyIncludes(includes);

                return query
                    .OrderBy(x => x.CriterionSequence)
                    .ToList();
            }
        }

        public static int CountRubricCriteria(Guid rubricIdentifier)
        {
            using (var context = CreateContext())
            {
                return context.TRubricCriteria
                    .Where(x => x.RubricIdentifier == rubricIdentifier)
                    .Count();
            }
        }

        #endregion

        #region TRubricConnection

        public static List<QBankQuestion> SelectRubricQuestions(Guid rubricId)
        {
            using (var context = CreateContext())
                return QueryRubricBankQuestions(context, rubricId).OrderBy(x => x.QuestionText).ToList();
        }

        public static int CountRubricQuestions(Guid rubricId)
        {
            using (var context = CreateContext())
                return QueryRubricBankQuestions(context, rubricId).Count();
        }

        public static bool HasRubricAttempts(Guid rubricId)
        {
            using (var context = CreateContext())
                return QueryRubricAttemptQuestions(context, rubricId).Any();
        }

        public static Guid[] GetRubricAttempts(Guid rubricId)
        {
            using (var context = CreateContext())
                return QueryRubricAttemptQuestions(context, rubricId).Select(x => x.AttemptIdentifier).ToArray();
        }

        public static int CountRubricAttempts(Guid rubricId)
        {
            using (var context = CreateContext())
                return QueryRubricAttemptQuestions(context, rubricId).Select(x => x.AttemptIdentifier).Distinct().Count();
        }

        private static IQueryable<QBankQuestion> QueryRubricBankQuestions(InternalDbContext db, Guid rubricId)
        {
            return db.TRubricConnections
                .Where(x => x.RubricIdentifier == rubricId)
                .Join(db.BankQuestions,
                    a => a.QuestionIdentifier,
                    b => b.QuestionIdentifier,
                    (a, b) => b
                );
        }

        private static IQueryable<QAttemptQuestion> QueryRubricAttemptQuestions(InternalDbContext db, Guid rubricId)
        {
            return db.TRubricConnections
                .Where(x => x.RubricIdentifier == rubricId)
                .Join(db.QAttemptQuestions,
                    a => a.QuestionIdentifier,
                    b => b.QuestionIdentifier,
                    (a, b) => b
                );
        }

        #endregion

        #region Helpers

        static InternalDbContext CreateContext() => new InternalDbContext(false);

        #endregion
    }
}
