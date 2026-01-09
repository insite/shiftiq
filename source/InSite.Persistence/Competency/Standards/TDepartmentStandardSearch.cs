using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace InSite.Persistence
{
    public static class TDepartmentStandardSearch
    {
        #region Classes

        private class StandardPermissionReadHelper : ReadHelper<TDepartmentStandard>
        {
            public static readonly StandardPermissionReadHelper Instance = new StandardPermissionReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TDepartmentStandard>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Database.CommandTimeout = 60;

                    var query = context.TDepartmentStandards.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region SELECT

        public static TDepartmentStandard SelectFirst(
            Expression<Func<TDepartmentStandard, bool>> filter,
            params Expression<Func<TDepartmentStandard, object>>[] includes) =>
            StandardPermissionReadHelper.Instance.SelectFirst(filter, includes);

        public static T[] Bind<T>(
            Expression<Func<TDepartmentStandard, T>> binder,
            Expression<Func<TDepartmentStandard, bool>> filter,
            string modelSort = null,
            string entitySort = null) => StandardPermissionReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<TDepartmentStandard, bool>> filter) =>
            StandardPermissionReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<TDepartmentStandard, bool>> filter) =>
            StandardPermissionReadHelper.Instance.Exists(filter);

        #endregion

        public static DataTable SelectDepartmentProfilesOnly(Guid department)
        {
            const string query = @"
SELECT s.*
FROM standards.[Standard] AS s
INNER JOIN identities.TDepartmentStandard AS ds ON ds.StandardIdentifier = s.StandardIdentifier
WHERE
    ds.DepartmentIdentifier = @GroupIdentifier
ORDER BY
    s.ContentTitle";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("GroupIdentifier", department));
        }

        public static DataTable SelectNewDepartmentProfiles(Guid organization, Guid group, string searchText)
        {
            var query = new StringBuilder(@"
SELECT s.*
FROM standards.[Standard] AS s
WHERE s.OrganizationIdentifier = @OrganizationIdentifier
      AND s.StandardType = 'Profile'
      AND s.StandardIdentifier NOT IN
          (
              SELECT DS.StandardIdentifier
              FROM identities.TDepartmentStandard AS DS
              WHERE DS.DepartmentIdentifier = @GroupIdentifier
          )
");

            if (!string.IsNullOrEmpty(searchText))
                query.Append(" AND (s.ContentTitle LIKE @SearchText OR s.Code LIKE @SearchText)");

            query.Append("ORDER BY s.ContentTitle");

            return DatabaseHelper.CreateDataTable(query.ToString(), 
                new SqlParameter("OrganizationIdentifier", organization),
                new SqlParameter("GroupIdentifier", group),
                new SqlParameter("SearchText", string.Format("%{0}%", searchText))
                );
        }
    }
}
