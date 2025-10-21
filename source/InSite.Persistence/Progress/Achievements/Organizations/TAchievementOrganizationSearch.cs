using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class TAchievementOrganizationSearch
    {
        private class AchievementOrganizationReadHelper : ReadHelper<TAchievementOrganization>
        {
            public static readonly AchievementOrganizationReadHelper Instance = new AchievementOrganizationReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TAchievementOrganization>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TAchievementOrganizations.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TAchievementOrganization SelectFirst(Expression<Func<TAchievementOrganization, bool>> filter,
            params Expression<Func<TAchievementOrganization, object>>[] includes)
        {
            return AchievementOrganizationReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<TAchievementOrganization> Select(
            Expression<Func<TAchievementOrganization, bool>> filter,
            params Expression<Func<TAchievementOrganization, object>>[] includes)
        {
            return AchievementOrganizationReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<TAchievementOrganization> Select(
            Expression<Func<TAchievementOrganization, bool>> filter,
            string sortExpression,
            params Expression<Func<TAchievementOrganization, object>>[] includes)
        {
            return AchievementOrganizationReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<TAchievementOrganization, T>> binder,
            Expression<Func<TAchievementOrganization, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementOrganizationReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<TAchievementOrganization, T>> binder,
            Expression<Func<TAchievementOrganization, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementOrganizationReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<TAchievementOrganization, bool>> filter)
        {
            return AchievementOrganizationReadHelper.Instance.Count(filter);
        }

        public static bool Exists(Expression<Func<TAchievementOrganization, bool>> filter)
        {
            return AchievementOrganizationReadHelper.Instance.Exists(filter);
        }
    }
}
