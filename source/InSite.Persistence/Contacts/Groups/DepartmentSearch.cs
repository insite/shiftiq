using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class DepartmentSearch
    {
        public static T[] Bind<T>(
            Expression<Func<Department, T>> binder,
            Expression<Func<Department, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            DepartmentReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<Department, T>> binder,
            Expression<Func<Department, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            DepartmentReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static T[] Bind<T>(Expression<Func<Department, T>> binder, DepartmentFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .Select(binder)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        public static int Count(DepartmentFilter filter = null)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        public static int Count(Expression<Func<Department, bool>> filter)
            => DepartmentReadHelper.Instance.Count(filter);

        public static bool Exists(Guid department)
        {
            using (var db = new InternalDbContext())
                return db.Departments.Any(x => x.DepartmentIdentifier == department);
        }

        public static bool Exists(Expression<Func<Department, bool>> filter)
            => DepartmentReadHelper.Instance.Exists(filter);

        public static T[] Bind<T>(Expression<Func<Department, T>> binder, Expression<Func<Department, bool>> filter)
            => DepartmentReadHelper.Instance.Bind(binder, filter);

        public static T[] Bind<T>(
            Expression<Func<Department, T>> binder, 
            Expression<Func<Department, bool>> filter, 
            Paging paging,
            string modelSort = null, string entitySort = null)
            => DepartmentReadHelper.Instance.Bind(binder, filter, paging, modelSort, entitySort);

        public static Department SelectFirst(Expression<Func<Department, bool>> filter, params Expression<Func<Department, object>>[] includes)
            => DepartmentReadHelper.Instance.SelectFirst(filter, includes);

        public static Department[] SelectAll(Expression<Func<Department, bool>> filter, params Expression<Func<Department, object>>[] includes)
            => DepartmentReadHelper.Instance.Select(filter, includes);

        public static Department Select(Guid department, params Expression<Func<Department, object>>[] includes)
            => DepartmentReadHelper.Instance.SelectFirst(x => x.DepartmentIdentifier == department, includes);

        public static Department Select(Guid department)
        {
            using (var db = new InternalDbContext())
            {
                return db.Departments
                    .Include(x => x.Organization)
                    .FirstOrDefault(x => x.DepartmentIdentifier == department);
            }
        }

        public static List<string> SelectDepartmentLabels(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                return db.Departments
                    .Where(x => x.OrganizationIdentifier == organizationId && x.DepartmentLabel != null)
                    .Select(x => x.DepartmentLabel)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public static Department Select(string departmentName, Guid organizationId, string subtype = null)
        {
            using (var db = new InternalDbContext())
            {
                if (subtype == null)
                    return db.Departments.FirstOrDefault(x => x.DepartmentName == departmentName && x.OrganizationIdentifier == organizationId);

                return db.Departments.FirstOrDefault(x => x.DepartmentName == departmentName && x.OrganizationIdentifier == organizationId);
            }
        }

        public static List<CompanyDepartment> SelectCompanyDepartments(Guid organizationId, Guid? excludeId = null)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.CompanyDepartments.Where(x => x.OrganizationIdentifier == organizationId);

                if (excludeId.HasValue)
                    query = query.Where(x => x.DepartmentIdentifier != excludeId.Value);

                return query
                    .OrderBy(x => x.CompanyName)
                    .ThenBy(x => x.DepartmentName)
                    .ToList();
            }
        }

        #region Private

        private class DepartmentReadHelper : ReadHelper<Department>
        {
            public static readonly DepartmentReadHelper Instance = new DepartmentReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Department>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Departments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        private static IQueryable<Department> CreateQuery(DepartmentFilter filter, InternalDbContext db)
        {
            var query = db.Departments.AsQueryable();

            if (filter == null)
                return query;

            if (filter.OrganizationIdentifier != Guid.Empty)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.Organizations != null)
            {
                var organizations = new List<Guid>();

                foreach (var organization in filter.Organizations)
                    organizations.Add(organization.OrganizationIdentifier);

                query = query.Where(x => organizations.Contains(x.OrganizationIdentifier));
            }

            if (filter.DepartmentName.HasValue())
                query = query.Where(x => x.DepartmentName.Contains(filter.DepartmentName));

            if (filter.DepartmentCode.HasValue())
                query = query.Where(x => x.DepartmentCode.Contains(filter.DepartmentCode));

            if (filter.DivisionIdentifier.HasValue)
                query = query.Where(x => x.DivisionIdentifier == filter.DivisionIdentifier);

            if (filter.CreatedSince.HasValue)
                query = query.Where(x => x.GroupCreated >= filter.CreatedSince.Value);

            if (filter.CreatedBefore.HasValue)
                query = query.Where(x => x.GroupCreated < filter.CreatedBefore.Value);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(
                    x => db.Memberships.Any(
                        y => y.GroupIdentifier == x.DepartmentIdentifier
                          && y.UserIdentifier == filter.UserIdentifier));

            if (filter.DepartmentLabel.HasValue())
                query = query.Where(x => x.DepartmentLabel == filter.DepartmentLabel);

            if (filter.CompanyName.HasValue())
                query = query.Where(x => x.Organization.CompanyName.Contains(filter.CompanyName));

            return query;
        }

        #endregion
    }
}
