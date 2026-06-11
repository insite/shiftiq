using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsAchievementDepartmentSearch
    {
        private class AchievementDepartmentReadHelper : ReadHelper<VCmdsAchievementDepartment>
        {
            public static readonly AchievementDepartmentReadHelper Instance = new AchievementDepartmentReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VCmdsAchievementDepartment>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VCmdsAchievementDepartments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static VCmdsAchievementDepartment SelectFirst(Expression<Func<VCmdsAchievementDepartment, bool>> filter,
            params Expression<Func<VCmdsAchievementDepartment, object>>[] includes)
        {
            return AchievementDepartmentReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<VCmdsAchievementDepartment> Select(
            Expression<Func<VCmdsAchievementDepartment, bool>> filter,
            params Expression<Func<VCmdsAchievementDepartment, object>>[] includes)
        {
            return AchievementDepartmentReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<VCmdsAchievementDepartment> Select(
            Expression<Func<VCmdsAchievementDepartment, bool>> filter,
            string sortExpression,
            params Expression<Func<VCmdsAchievementDepartment, object>>[] includes)
        {
            return AchievementDepartmentReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<VCmdsAchievementDepartment, T>> binder,
            Expression<Func<VCmdsAchievementDepartment, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementDepartmentReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<VCmdsAchievementDepartment, T>> binder,
            Expression<Func<VCmdsAchievementDepartment, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementDepartmentReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<VCmdsAchievementDepartment, bool>> filter)
        {
            return AchievementDepartmentReadHelper.Instance.Count(filter);
        }
    }
}
