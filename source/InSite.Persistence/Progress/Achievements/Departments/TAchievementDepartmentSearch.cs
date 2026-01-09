using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence
{
    public static class TAchievementDepartmentSearch
    {
        private class AchievementDepartmentReadHelper : ReadHelper<TAchievementDepartment>
        {
            public static readonly AchievementDepartmentReadHelper Instance = new AchievementDepartmentReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TAchievementDepartment>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TAchievementDepartments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TAchievementDepartment SelectFirst(Expression<Func<TAchievementDepartment, bool>> filter,
            params Expression<Func<TAchievementDepartment, object>>[] includes)
        {
            return AchievementDepartmentReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<TAchievementDepartment> Select(
            Expression<Func<TAchievementDepartment, bool>> filter,
            params Expression<Func<TAchievementDepartment, object>>[] includes)
        {
            return AchievementDepartmentReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<TAchievementDepartment> Select(
            Expression<Func<TAchievementDepartment, bool>> filter,
            string sortExpression,
            params Expression<Func<TAchievementDepartment, object>>[] includes)
        {
            return AchievementDepartmentReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<TAchievementDepartment, T>> binder,
            Expression<Func<TAchievementDepartment, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementDepartmentReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<TAchievementDepartment, T>> binder,
            Expression<Func<TAchievementDepartment, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementDepartmentReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<TAchievementDepartment, bool>> filter)
        {
            return AchievementDepartmentReadHelper.Instance.Count(filter);
        }

        public static bool Exists(Expression<Func<TAchievementDepartment, bool>> filter)
        {
            return AchievementDepartmentReadHelper.Instance.Exists(filter);
        }

    }
}
