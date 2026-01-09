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
    public static class StandardValidationSearch
    {
        #region Classes

        private class StandardValidationReadHelper : ReadHelper<StandardValidation>
        {
            public static readonly StandardValidationReadHelper Instance = new StandardValidationReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<StandardValidation>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Database.CommandTimeout = 5 * 60;

                    var query = context.StandardValidations.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region SELECT

        public static IReadOnlyList<StandardValidation> Select(
            Expression<Func<StandardValidation, bool>> filter,
            params Expression<Func<StandardValidation, object>>[] includes) =>
            StandardValidationReadHelper.Instance.Select(filter, includes);

        public static StandardValidation SelectFirst(
            Expression<Func<StandardValidation, bool>> filter,
            params Expression<Func<StandardValidation, object>>[] includes) =>
            StandardValidationReadHelper.Instance.SelectFirst(filter, includes);

        public static DataTable Select(StandardValidationFilter filter)
        {
            using (var context = new InternalDbContext())
            {
                context.Database.CommandTimeout = 5 * 60;

                var query = context.StandardValidations.AsQueryable().AsNoTracking();

                var selectQuery =
                    FilterQuery(filter, query)
                    .Select(x => new
                    {
                        x.ValidationIdentifier,
                        x.StandardIdentifier,
                        StandardName = x.Standard.ContentTitle,
                        x.Standard.StandardType,
                        StandardCode = x.Standard.Code,
                        x.UserIdentifier,
                        UserFullName = x.User.FullName,
                        UserEmail = x.User.Email,
                        x.ValidationStatus,
                        x.Expired,
                        x.ValidationDate,
                        x.ValidatorUserIdentifier,
                        ValidatorUserFullName = x.Validator.FullName,
                        x.IsValidated,
                        x.Created
                    });

                return selectQuery
                    .OrderByDescending(x => x.Created)
                    .ApplyPaging(filter)
                    .ToDataTable();
            }
        }

        public static T[] Bind<T>(
            Expression<Func<StandardValidation, T>> binder,
            Expression<Func<StandardValidation, bool>> filter,
            string modelSort = null,
            string entitySort = null) => StandardValidationReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] Bind<T>(Expression<Func<StandardValidation, T>> binder, StandardValidationFilter filter)
        {
            return StandardValidationReadHelper.Instance.Bind(
                (IQueryable<StandardValidation> query) => query.Select(binder),
                (IQueryable<StandardValidation> query) => FilterQuery(filter, query),
                filter.Paging,
                filter.OrderBy, null);
        }

        public static int Count(StandardValidationFilter filter) =>
            StandardValidationReadHelper.Instance.Count(query => FilterQuery(filter, query));

        public static int Count(Expression<Func<StandardValidation, bool>> filter) =>
            StandardValidationReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<StandardValidation, bool>> filter) =>
            StandardValidationReadHelper.Instance.Exists(filter);

        private static IQueryable<StandardValidation> FilterQuery(StandardValidationFilter filter, IQueryable<StandardValidation> query)
        {
            query = query.Where(
                x => x.User.Memberships.Any(
                    y => y.Group.GroupType == GroupTypes.Department
                      && (y.Group.OrganizationIdentifier == filter.OrganizationIdentifier)
                      && (y.MembershipType == "Company" || y.MembershipType == "Department")));

            if (filter.DepartmentIdentifier.HasValue)
                query = query.Where(x => x.User.Memberships.Any(y => y.GroupIdentifier == filter.DepartmentIdentifier));

            if (filter.StandardIdentifier.HasValue)
                query = query.Where(x => x.StandardIdentifier == filter.StandardIdentifier.Value);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier.Value);

            if (filter.ValidatorUserIdentifier.HasValue)
                query = query.Where(x => x.ValidatorUserIdentifier == filter.ValidatorUserIdentifier);

            if (filter.SelfAssessmentStatus.HasValue())
                query = query.Where(x => x.SelfAssessmentStatus == filter.SelfAssessmentStatus);

            if (filter.StandardType.HasValue())
                query = query.Where(x => x.Standard.StandardType == filter.StandardType);

            if (filter.ValidationStatus.HasValue())
                query = query.Where(x => x.ValidationStatus == filter.ValidationStatus);

            return query;
        }

        #endregion
    }
}
