using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class StandardOrganizationSearch
    {
        #region Classes

        private class StandardOrganizationReadHelper : ReadHelper<StandardOrganization>
        {
            public static readonly StandardOrganizationReadHelper Instance = new StandardOrganizationReadHelper();

            public T[] Bind<T>(
                InternalDbContext context,
                Expression<Func<StandardOrganization, T>> binder,
                Expression<Func<StandardOrganization, bool>> filter,
                string modelSort = null,
                string entitySort = null)
            {
                var query = context.StandardOrganizations.AsQueryable().AsNoTracking();
                var modelQuery = BuildQuery(query, binder, filter, null, modelSort, entitySort, false);

                return modelQuery.ToArray();
            }

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<StandardOrganization>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.StandardOrganizations.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Binding

        public static StandardOrganization SelectFirst(Expression<Func<StandardOrganization, bool>> filter, params Expression<Func<StandardOrganization, object>>[] includes) =>
            StandardOrganizationReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<StandardOrganization> Select(
            Expression<Func<StandardOrganization, bool>> filter,
            params Expression<Func<StandardOrganization, object>>[] includes) => StandardOrganizationReadHelper.Instance.Select(filter, includes);

        public static IReadOnlyList<StandardOrganization> Select(
            Expression<Func<StandardOrganization, bool>> filter,
            string sortExpression,
            params Expression<Func<StandardOrganization, object>>[] includes) => StandardOrganizationReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T[] Bind<T>(
            Expression<Func<StandardOrganization, T>> binder,
            Expression<Func<StandardOrganization, bool>> filter,
            string modelSort = null,
            string entitySort = null) => StandardOrganizationReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<StandardOrganization, T>> binder,
            Expression<Func<StandardOrganization, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            StandardOrganizationReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<StandardOrganization, bool>> filter) => StandardOrganizationReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<StandardOrganization, bool>> filter) =>
            StandardOrganizationReadHelper.Instance.Exists(filter);

        #endregion

        #region SELECT

        public static StandardOrganization Select(Guid organizationId, Guid assetId)
        {
            using (var db = new InternalDbContext())
            {
                return db.StandardOrganizations.FirstOrDefault(x => x.OrganizationIdentifier == organizationId && x.StandardIdentifier == assetId);
            }
        }

        #endregion
    }
}
