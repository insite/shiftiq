using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsAchievementOrganizationSearch
    {
        private class AchievementOrganizationReadHelper : ReadHelper<VCmdsAchievementOrganization>
        {
            public static readonly AchievementOrganizationReadHelper Instance = new AchievementOrganizationReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VCmdsAchievementOrganization>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VCmdsAchievementOrganizations.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static VCmdsAchievementOrganization SelectFirst(Expression<Func<VCmdsAchievementOrganization, bool>> filter,
            params Expression<Func<VCmdsAchievementOrganization, object>>[] includes)
        {
            return AchievementOrganizationReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<VCmdsAchievementOrganization> Select(
            Expression<Func<VCmdsAchievementOrganization, bool>> filter,
            params Expression<Func<VCmdsAchievementOrganization, object>>[] includes)
        {
            return AchievementOrganizationReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<VCmdsAchievementOrganization> Select(
            Expression<Func<VCmdsAchievementOrganization, bool>> filter,
            string sortExpression,
            params Expression<Func<VCmdsAchievementOrganization, object>>[] includes)
        {
            return AchievementOrganizationReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<VCmdsAchievementOrganization, T>> binder,
            Expression<Func<VCmdsAchievementOrganization, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementOrganizationReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<VCmdsAchievementOrganization, T>> binder,
            Expression<Func<VCmdsAchievementOrganization, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementOrganizationReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<VCmdsAchievementOrganization, bool>> filter)
        {
            return AchievementOrganizationReadHelper.Instance.Count(filter);
        }
    }
}
