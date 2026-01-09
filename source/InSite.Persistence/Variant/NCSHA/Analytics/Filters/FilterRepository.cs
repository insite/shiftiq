using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class FilterRepository
    {
        #region Classes

        private class FilterReadHelper : ReadHelper<Filter>
        {
            public static readonly FilterReadHelper Instance = new FilterReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Filter>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Filters.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Read

        public static T[] Bind<T>(
            Expression<Func<Filter, T>> binder,
            Expression<Func<Filter, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return FilterReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<Filter, T>> binder,
            Expression<Func<Filter, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return FilterReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static bool Exists(Expression<Func<Filter, bool>> filter) =>
            FilterReadHelper.Instance.Exists(filter);

        public static int Count(Expression<Func<Filter, bool>> filter) =>
            FilterReadHelper.Instance.Count(filter);

        #endregion

        #region Write

        public static Filter Save(Filter filter)
        {
            using (var db = new InternalDbContext())
            {
                var existId = db.Filters
                    .Where(x => x.AuthorUserIdentifier == filter.AuthorUserIdentifier && x.FilterName == filter.FilterName)
                    .Select(x => (Guid?)x.FilterId)
                    .FirstOrDefault();

                if (existId.HasValue)
                {
                    filter.FilterId = existId.Value;
                    db.Entry(filter).State = EntityState.Modified;
                }
                else
                    db.Filters.Add(filter);

                db.SaveChanges();

                return filter;
            }
        }

        #endregion

        #region DELETE

        public static void Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var entity = new Filter { FilterId = id };
                db.Filters.Attach(entity);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion
    }
}
