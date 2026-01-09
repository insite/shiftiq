using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class StandardValidationChangeSearch
    {
        #region Classes

        private class CompetencyValidationReadHelper : ReadHelper<StandardValidationChange>
        {
            public static readonly CompetencyValidationReadHelper Instance = new CompetencyValidationReadHelper();

            public T[] Bind<T>(
                InternalDbContext context,
                Expression<Func<StandardValidationChange, T>> binder,
                Expression<Func<StandardValidationChange, bool>> filter,
                string modelSort = null,
                string entitySort = null)
            {
                var query = context.StandardValidationChanges.AsQueryable().AsNoTracking();
                var modelQuery = BuildQuery(query, binder, filter, null, modelSort, entitySort, false);

                return modelQuery.ToArray();
            }

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<StandardValidationChange>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.StandardValidationChanges.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Binding

        public static StandardValidationChange Select(Guid id, params Expression<Func<StandardValidationChange, object>>[] includes) =>
            CompetencyValidationReadHelper.Instance.SelectFirst(x => x.ChangeIdentifier == id, includes);

        public static StandardValidationChange SelectFirst(Expression<Func<StandardValidationChange, bool>> filter, params Expression<Func<StandardValidationChange, object>>[] includes) =>
            CompetencyValidationReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<StandardValidationChange> Select(
            Expression<Func<StandardValidationChange, bool>> filter,
            params Expression<Func<StandardValidationChange, object>>[] includes) => CompetencyValidationReadHelper.Instance.Select(filter, includes);

        public static IReadOnlyList<StandardValidationChange> Select(
            Expression<Func<StandardValidationChange, bool>> filter,
            string sortExpression,
            params Expression<Func<StandardValidationChange, object>>[] includes) => CompetencyValidationReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T[] Bind<T>(
            Expression<Func<StandardValidationChange, T>> binder,
            Expression<Func<StandardValidationChange, bool>> filter,
            string modelSort = null,
            string entitySort = null) => CompetencyValidationReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<StandardValidationChange, T>> binder,
            Expression<Func<StandardValidationChange, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            CompetencyValidationReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<StandardValidationChange, bool>> filter) => CompetencyValidationReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<StandardValidationChange, bool>> filter) =>
            CompetencyValidationReadHelper.Instance.Exists(filter);

        #endregion

        public static List<CompetencyValidationSummary> SelectStatusHistory(Guid competencyId, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.CompetencyValidationSummaries
                    .Where(x => x.StandardIdentifier == competencyId && x.UserIdentifier == user)
                    .OrderByDescending(x => x.ChangePosted)
                    .ToList();
            }
        }

        public static int Count(StandardValidationChangeFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        public static DataTable Select(StandardValidationChangeFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        ChangeIdentifier = x.ChangeIdentifier,
                        StandardIdentifier = x.StandardIdentifier,
                        UserIdentifier = x.UserIdentifier,
                        AuthorUserIdentifier = x.AuthorUserIdentifier,
                        ChangeComment = x.ChangeComment,
                        ChangePosted = x.ChangePosted,
                        ChangeStatus = x.ChangeStatus,
                        StandardCode = x.Standard.Code,
                        StandardTitle = x.Standard.ContentTitle,
                        UserFullName = x.User.FullName,
                        AuthorFullName = x.Author.FullName
                    })
                    .OrderByDescending(x => x.ChangePosted)
                    .ApplyPaging(filter)
                    .ToDataTable();
            }
        }

        private static IQueryable<StandardValidationChange> CreateQueryByFilter(StandardValidationChangeFilter filter, InternalDbContext db)
        {
            var query = db.StandardValidationChanges.AsQueryable();

            query = query.Where(
                x => x.User.Memberships.Any(
                    y => y.Group.GroupType == GroupTypes.Department
                      && (y.Group.OrganizationIdentifier == filter.OrganizationIdentifier)
                      && (y.MembershipType == "Company" || y.MembershipType == "Department")));

            if (filter.DepartmentIdentifier.HasValue)
                query = query.Where(x => x.User.Memberships.Any(y => y.GroupIdentifier == filter.DepartmentIdentifier));

            if (filter.StandardIdentifier.HasValue)
                query = query.Where(x => x.StandardIdentifier == filter.StandardIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier.Value);

            if (filter.ValidatorUserIdentifier.HasValue)
                query = query.Where(x => x.AuthorUserIdentifier == filter.ValidatorUserIdentifier);

            if (filter.StandardType.HasValue())
                query = query.Where(x => x.Standard.StandardType == filter.StandardType);

            if (filter.ValidationStatus.HasValue())
                query = query.Where(x => x.ChangeStatus == filter.ValidationStatus);

            if (filter.ChangePostedSince.HasValue)
                query = query.Where(x => x.ChangePosted >= filter.ChangePostedSince.Value);

            if (filter.ChangePostedBefore.HasValue)
                query = query.Where(x => x.ChangePosted < filter.ChangePostedBefore.Value);

            return query;
        }
    }
}
