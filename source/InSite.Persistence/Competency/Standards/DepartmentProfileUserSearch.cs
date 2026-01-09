using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class DepartmentProfileUserSearch
    {
        #region Classes

        private class DepartmentProfileUserReadHelper : ReadHelper<DepartmentProfileUser>
        {
            public static readonly DepartmentProfileUserReadHelper Instance = new DepartmentProfileUserReadHelper();

            public T[] Bind<T>(
                InternalDbContext context,
                Expression<Func<DepartmentProfileUser, T>> binder,
                Expression<Func<DepartmentProfileUser, bool>> filter,
                string modelSort = null,
                string entitySort = null)
            {
                var query = context.DepartmentProfileUsers.AsQueryable().AsNoTracking();
                var modelQuery = BuildQuery(query, binder, filter, null, modelSort, entitySort, false);

                return modelQuery.ToArray();
            }

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<DepartmentProfileUser>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.DepartmentProfileUsers.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Binding

        public static DepartmentProfileUser Select(Guid id, params Expression<Func<DepartmentProfileUser, object>>[] includes) =>
            DepartmentProfileUserReadHelper.Instance.SelectFirst(x => x.UserIdentifier == id, includes);

        public static DepartmentProfileUser SelectFirst(Expression<Func<DepartmentProfileUser, bool>> filter, params Expression<Func<DepartmentProfileUser, object>>[] includes) =>
            DepartmentProfileUserReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<DepartmentProfileUser> Select(
            Expression<Func<DepartmentProfileUser, bool>> filter,
            params Expression<Func<DepartmentProfileUser, object>>[] includes) => DepartmentProfileUserReadHelper.Instance.Select(filter, includes);

        public static IReadOnlyList<DepartmentProfileUser> Select(
            Expression<Func<DepartmentProfileUser, bool>> filter,
            string sortExpression,
            params Expression<Func<DepartmentProfileUser, object>>[] includes) => DepartmentProfileUserReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T[] Bind<T>(
            Expression<Func<DepartmentProfileUser, T>> binder,
            Expression<Func<DepartmentProfileUser, bool>> filter,
            string modelSort = null,
            string entitySort = null) => DepartmentProfileUserReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<DepartmentProfileUser, T>> binder,
            Expression<Func<DepartmentProfileUser, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            DepartmentProfileUserReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<DepartmentProfileUser, bool>> filter) => DepartmentProfileUserReadHelper.Instance.Count(filter);

        public static bool Exists(Guid id) =>
            DepartmentProfileUserReadHelper.Instance.Exists(x => x.UserIdentifier == id);

        public static bool Exists(Expression<Func<DepartmentProfileUser, bool>> filter) =>
            DepartmentProfileUserReadHelper.Instance.Exists(filter);

        #endregion

        #region Filter

        public static int Count(DepartmentProfileUserFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        public static DataTable Select(DepartmentProfileUserFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        DepartmentIdentifier = x.DepartmentIdentifier,
                        ProfileStandardIdentifier = x.ProfileStandardIdentifier,
                        UserIdentifier = x.UserIdentifier,
                        OrganizationName = x.Department.Organization.CompanyName,
                        DepartmentName = x.Department.DepartmentName,
                        ProfileName = x.Profile.ContentTitle,
                        UserFullName = x.User.FullName
                    })
                    .OrderBy("OrganizationName, DepartmentName, ProfileName, UserFullName")
                    .ApplyPaging(filter)
                    .ToDataTable();
            }
        }

        private static IQueryable<DepartmentProfileUser> CreateQueryByFilter(DepartmentProfileUserFilter filter, InternalDbContext db)
        {
            var query = db.DepartmentProfileUsers.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Department.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.DepartmentIdentifier.HasValue)
                query = query.Where(x => x.DepartmentIdentifier == filter.DepartmentIdentifier);

            if (filter.ProfileStandardIdentifier.HasValue)
                query = query.Where(x => x.ProfileStandardIdentifier == filter.ProfileStandardIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            return query;
        }

        #endregion
    }
}
