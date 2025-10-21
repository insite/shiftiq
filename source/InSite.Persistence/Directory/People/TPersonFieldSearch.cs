using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

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
    }
}
