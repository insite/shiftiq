using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public class TAchievementClassificationSearch
    {
        private class AchievementClassificationReadHelper : ReadHelper<VAchievementClassification>
        {
            public static readonly AchievementClassificationReadHelper Instance = new AchievementClassificationReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VAchievementClassification>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VAchievementClassifications.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static VAchievementClassification SelectFirst(Expression<Func<VAchievementClassification, bool>> filter,
            params Expression<Func<VAchievementClassification, object>>[] includes)
        {
            return AchievementClassificationReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<VAchievementClassification> Select(
            Expression<Func<VAchievementClassification, bool>> filter,
            params Expression<Func<VAchievementClassification, object>>[] includes)
        {
            return AchievementClassificationReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<VAchievementClassification> Select(
            Expression<Func<VAchievementClassification, bool>> filter,
            string sortExpression,
            params Expression<Func<VAchievementClassification, object>>[] includes)
        {
            return AchievementClassificationReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<VAchievementClassification, T>> binder,
            Expression<Func<VAchievementClassification, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementClassificationReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<VAchievementClassification, T>> binder,
            Expression<Func<VAchievementClassification, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementClassificationReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<VAchievementClassification, bool>> filter)
        {
            return AchievementClassificationReadHelper.Instance.Count(filter);
        }

        public static bool Exists(Expression<Func<VAchievementClassification, bool>> filter)
        {
            return AchievementClassificationReadHelper.Instance.Exists(filter);
        }
    }
}
