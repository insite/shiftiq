using System;
using System.Linq;
using System.Linq.Expressions;

using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public abstract class ReadHelper<TEntity>
    {
        #region Abstract methods

        protected abstract TResult ExecuteQuery<TResult>(Func<IQueryable<TEntity>, TResult> func);

        #endregion

        #region SELECT

        public TEntity[] Select(
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, object>>[] includes)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, includes, null, null, false).ToArray());
        }

        public TEntity[] Select(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, include, null, null, null, false).ToArray());
        }

        public TEntity[] Select(
            Expression<Func<TEntity, bool>> filter,
            string sortExpression,
            Expression<Func<TEntity, object>>[] includes)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, includes, null, sortExpression, false).ToArray());
        }

        public TEntity[] Select(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            string sortExpression,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, include, null, null, sortExpression, false).ToArray());
        }

        public TEntity SelectFirst(
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, object>>[] includes)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, includes, null, null, false).FirstOrDefault());
        }

        public TEntity SelectFirst(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, include, null, null, null, false).FirstOrDefault());
        }

        public TEntity SelectFirst(
            Expression<Func<TEntity, bool>> filter,
            string sortExpression,
            Expression<Func<TEntity, object>>[] includes)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, includes, null, sortExpression, false).FirstOrDefault());
        }

        public TEntity SelectFirst(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            string sortExpression,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            return ExecuteQuery(query => BuildQuery(query, x => x, filter, include, null, null, sortExpression, false).FirstOrDefault());
        }

        public T[] Bind<T>(
            Expression<Func<TEntity, T>> binder,
            Expression<Func<TEntity, bool>> filter,
            Paging paging,
            string modelSort,
            string entitySort
            )
        {
            return ExecuteQuery(query =>
            {
                query = query.Where(filter);
                if (!string.IsNullOrEmpty(entitySort))
                    query = query.OrderBy(entitySort);

                var modelQuery = query.Select(binder);
                if (!string.IsNullOrEmpty(modelSort))
                    modelQuery = modelQuery.OrderBy(modelSort);

                return modelQuery
                    .ApplyPaging(paging)
                    .ToArray();
            });
        }

        public T[] Bind<T>(
            Expression<Func<TEntity, T>> binder,
            Expression<Func<TEntity, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ExecuteQuery(query => BuildQuery(query, binder, filter, null, modelSort, entitySort, false).ToArray());
        }

        public T[] Bind<T>(
            Func<IQueryable<TEntity>, IQueryable<T>> bind,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ExecuteQuery(query => BuildQuery(query, bind, filter, q => q, null, modelSort, entitySort, false).ToArray());
        }

        public T[] Bind<T>(
            Func<IQueryable<TEntity>, IQueryable<T>> bind,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            Paging paging,
            string modelSort = null,
            string entitySort = null) => ExecuteQuery(query => BuildQuery(query, bind, filter, q => q, paging, modelSort, entitySort, false).ToArray());

        public T BindFirst<T>(
            Expression<Func<TEntity, T>> binder,
            Expression<Func<TEntity, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ExecuteQuery(query => BuildQuery(query, binder, filter, null, modelSort, entitySort, false).FirstOrDefault());
        }

        public T BindFirst<T>(
            Func<IQueryable<TEntity>, IQueryable<T>> bind,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ExecuteQuery(query => BuildQuery(query, bind, filter, q => q, null, modelSort, entitySort, false).FirstOrDefault());
        }

        public T[] Distinct<T>(
            Expression<Func<TEntity, T>> binder,
            Expression<Func<TEntity, bool>> filter,
            string modelSort)
        {
            return ExecuteQuery(query => BuildQuery(query, binder, filter, null, modelSort, null, true).ToArray());
        }

        public T[] Distinct<T>(
            Func<IQueryable<TEntity>, IQueryable<T>> bind,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            string modelSort)
        {
            return ExecuteQuery(query => BuildQuery(query, bind, filter, q => q, null, modelSort, null, true).ToArray());
        }

        public int Count(Expression<Func<TEntity, bool>> filter)
        {
            return Count(query => filter != null ? query.Where(filter) : query);
        }

        public int Count(Func<IQueryable<TEntity>, IQueryable<TEntity>> filter)
        {
            return ExecuteQuery(query => filter(query).Count());
        }

        public bool Exists(Expression<Func<TEntity, bool>> filter)
        {
            return Exists(query => filter != null ? query.Where(filter) : query);
        }

        public bool Exists(Func<IQueryable<TEntity>, IQueryable<TEntity>> filter)
        {
            return ExecuteQuery(query => filter(query).Any());
        }

        protected IQueryable<T> BuildQuery<T>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, T>> binder,
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, object>>[] includes,
            string modelSort,
            string entitySort,
            bool distinct)
        {
            return BuildQuery(
                query,
                q => q.Select(binder),
                q => filter != null ? q.Where(filter) : q,
                q => q.ApplyIncludes(includes),
                null,
                modelSort,
                entitySort,
                distinct);
        }

        protected IQueryable<T> BuildQuery<T>(
            IQueryable<TEntity> query,
            Func<IQueryable<TEntity>, IQueryable<T>> bind,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            Paging paging,
            string modelSort,
            string entitySort,
            bool distinct)
        {
            query = filter(include(query));

            if (entitySort.IsNotEmpty())
                query = query.OrderBy(entitySort);

            var modelQuery = bind(query);

            if (distinct)
                modelQuery = modelQuery.Distinct();

            if (modelSort.IsNotEmpty())
                modelQuery = modelQuery.OrderBy(modelSort);

            modelQuery = modelQuery.ApplyPaging(paging);

            return modelQuery;
        }

        #endregion
    }
}
