using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence
{
    public static class TAchievementStandardSearch
    {
        private class TAchievementStandardReadHelper : ReadHelper<TAchievementStandard>
        {
            public static readonly TAchievementStandardReadHelper Instance = new TAchievementStandardReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TAchievementStandard>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TAchievementStandards.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TAchievementStandard SelectFirst(Expression<Func<TAchievementStandard, bool>> filter,
            params Expression<Func<TAchievementStandard, object>>[] includes)
        {
            return TAchievementStandardReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<TAchievementStandard> Select(
            Expression<Func<TAchievementStandard, bool>> filter,
            params Expression<Func<TAchievementStandard, object>>[] includes)
        {
            return TAchievementStandardReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<TAchievementStandard> Select(
            Expression<Func<TAchievementStandard, bool>> filter,
            string sortExpression,
            params Expression<Func<TAchievementStandard, object>>[] includes)
        {
            return TAchievementStandardReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<TAchievementStandard, T>> binder,
            Expression<Func<TAchievementStandard, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return TAchievementStandardReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<TAchievementStandard, T>> binder,
            Expression<Func<TAchievementStandard, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return TAchievementStandardReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<TAchievementStandard, bool>> filter)
        {
            return TAchievementStandardReadHelper.Instance.Count(filter);
        }

        public static bool Exists(Expression<Func<TAchievementStandard, bool>> filter)
        {
            return TAchievementStandardReadHelper.Instance.Exists(filter);
        }
    }
}
