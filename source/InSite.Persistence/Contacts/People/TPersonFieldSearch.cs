using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class TPersonFieldSearch
    {
        private class TPersonFieldReadHelper : ReadHelper<TPersonField>
        {
            public static readonly TPersonFieldReadHelper Instance = new TPersonFieldReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TPersonField>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TPersonFields.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TPersonField[] Select(Expression<Func<TPersonField, bool>> predicate, string sortExpression)
        {
            return TPersonFieldReadHelper.Instance.Select(predicate, sortExpression, null);
        }
        public static int Count(TPersonFieldFilter filter) =>
            TPersonFieldReadHelper.Instance.Count(
                (IQueryable<TPersonField> query) => query.Filter(filter));

        public static IList<T> Bind<T>(
            Expression<Func<TPersonField, T>> binder,
            TPersonFieldFilter filter,
            string modelSort = null, string entitySort = null)
        {
            return TPersonFieldReadHelper.Instance.Bind(
                (IQueryable<TPersonField> query) => query.Select(binder),
                (IQueryable<TPersonField> query) => query.Filter(filter),
                modelSort, entitySort);
        }

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<TPersonField, T>> binder,
            Expression<Func<TPersonField, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            TPersonFieldReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static bool IsHideSkillsCheckLearnerBanner(Guid organizationId, Guid userId)
        {
            var value = GetValue(organizationId, userId, TPersonFieldName.HideSkillsCheckLearnerBanner);
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        public static int GetSkillsCheckPurchasedCount(Guid organizationId, Guid userId)
        {
            var value = GetValue(organizationId, userId, TPersonFieldName.SkillsCheckPurchasedCount);
            return int.TryParse(value, out var intValue) ? intValue : 0;
        }

        private static string GetValue(Guid organizationId, Guid userId, string fieldName)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TPersonFields
                    .Where(x => x.OrganizationIdentifier == organizationId && x.UserIdentifier == userId && x.FieldName == fieldName)
                    .FirstOrDefault();

                return existing?.FieldValue;
            }
        }
    }
}
