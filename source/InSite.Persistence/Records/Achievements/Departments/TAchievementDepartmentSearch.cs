using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;



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

        /// <summary>
        /// A department can hold only achievements owned by its own organization or by the
        /// partition (Global) organization.
        /// </summary>
        public static bool IsPermittedOwner(Guid achievementOrganizationId, Guid departmentOrganizationId)
        {
            return achievementOrganizationId == departmentOrganizationId
                || achievementOrganizationId == ServiceLocator.Partition.Identifier;
        }

        /// <summary>
        /// Returns the links that satisfy <see cref="IsPermittedOwner"/>. A link to an achievement
        /// or department that no longer exists is not permitted either.
        /// </summary>
        public static List<TAchievementDepartment> SelectPermitted(IEnumerable<TAchievementDepartment> links)
        {
            var list = links.ToList();
            if (list.Count == 0)
                return list;

            var achievementIds = list.Select(x => x.AchievementIdentifier).Distinct().ToArray();
            var departmentIds = list.Select(x => x.DepartmentIdentifier).Distinct().ToArray();

            Dictionary<Guid, Guid> achievementOwners, departmentOrganizations;

            using (var db = new InternalDbContext())
            {
                achievementOwners = db.QAchievements
                    .AsNoTracking()
                    .Where(x => achievementIds.Contains(x.AchievementIdentifier))
                    .Select(x => new { x.AchievementIdentifier, x.OrganizationIdentifier })
                    .ToDictionary(x => x.AchievementIdentifier, x => x.OrganizationIdentifier);

                departmentOrganizations = db.Departments
                    .AsNoTracking()
                    .Where(x => departmentIds.Contains(x.DepartmentIdentifier))
                    .Select(x => new { x.DepartmentIdentifier, x.OrganizationIdentifier })
                    .ToDictionary(x => x.DepartmentIdentifier, x => x.OrganizationIdentifier);
            }

            var permitted = new List<TAchievementDepartment>();

            foreach (var link in list)
            {
                var hasAchievement = achievementOwners.TryGetValue(link.AchievementIdentifier, out var achievementOrganizationId);
                var hasDepartment = departmentOrganizations.TryGetValue(link.DepartmentIdentifier, out var departmentOrganizationId);
                if (!hasAchievement || !hasDepartment)
                    continue;

                var isPermitted = IsPermittedOwner(achievementOrganizationId, departmentOrganizationId);
                if (isPermitted)
                    permitted.Add(link);
            }

            return permitted;
        }
    }
}
