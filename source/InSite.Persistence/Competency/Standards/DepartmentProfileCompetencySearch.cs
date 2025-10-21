using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence
{
    public static class DepartmentProfileCompetencySearch
    {
        #region Classes

        private class DepartmentProfileCompetencyReadHelper : ReadHelper<DepartmentProfileCompetency>
        {
            public static readonly DepartmentProfileCompetencyReadHelper Instance = new DepartmentProfileCompetencyReadHelper();

            public T[] Bind<T>(
                InternalDbContext context,
                Expression<Func<DepartmentProfileCompetency, T>> binder,
                Expression<Func<DepartmentProfileCompetency, bool>> filter,
                string modelSort = null,
                string entitySort = null)
            {
                var query = context.DepartmentProfileCompetencies.AsQueryable().AsNoTracking();
                var modelQuery = BuildQuery(query, binder, filter, null, modelSort, entitySort, false);

                return modelQuery.ToArray();
            }

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<DepartmentProfileCompetency>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.DepartmentProfileCompetencies.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Binding

        public static DepartmentProfileCompetency Select(Guid id, params Expression<Func<DepartmentProfileCompetency, object>>[] includes) =>
            DepartmentProfileCompetencyReadHelper.Instance.SelectFirst(x => x.CompetencyStandardIdentifier == id, includes);

        public static DepartmentProfileCompetency SelectFirst(Expression<Func<DepartmentProfileCompetency, bool>> filter, params Expression<Func<DepartmentProfileCompetency, object>>[] includes) =>
            DepartmentProfileCompetencyReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<DepartmentProfileCompetency> Select(
            Expression<Func<DepartmentProfileCompetency, bool>> filter,
            params Expression<Func<DepartmentProfileCompetency, object>>[] includes) => DepartmentProfileCompetencyReadHelper.Instance.Select(filter, includes);

        public static IReadOnlyList<DepartmentProfileCompetency> Select(
            Expression<Func<DepartmentProfileCompetency, bool>> filter,
            string sortExpression,
            params Expression<Func<DepartmentProfileCompetency, object>>[] includes) => DepartmentProfileCompetencyReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T[] Bind<T>(
            Expression<Func<DepartmentProfileCompetency, T>> binder,
            Expression<Func<DepartmentProfileCompetency, bool>> filter,
            string modelSort = null,
            string entitySort = null) => DepartmentProfileCompetencyReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<DepartmentProfileCompetency, T>> binder,
            Expression<Func<DepartmentProfileCompetency, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            DepartmentProfileCompetencyReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<DepartmentProfileCompetency, bool>> filter) => DepartmentProfileCompetencyReadHelper.Instance.Count(filter);

        public static bool Exists(Guid id) =>
            DepartmentProfileCompetencyReadHelper.Instance.Exists(x => x.CompetencyStandardIdentifier == id);

        public static bool Exists(Expression<Func<DepartmentProfileCompetency, bool>> filter) =>
            DepartmentProfileCompetencyReadHelper.Instance.Exists(filter);

        #endregion
    }
}
