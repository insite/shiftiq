using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence
{
    public static class StandardClassificationSearch
    {
        #region Classes

        private class StandardClassificationReadHelper : ReadHelper<StandardClassification>
        {
            public static readonly StandardClassificationReadHelper Instance = new StandardClassificationReadHelper();

            public T[] Bind<T>(
                InternalDbContext context,
                Expression<Func<StandardClassification, T>> binder,
                Expression<Func<StandardClassification, bool>> filter,
                string modelSort = null,
                string entitySort = null)
            {
                var query = context.StandardClassifications.AsQueryable().AsNoTracking();
                var modelQuery = BuildQuery(query, binder, filter, null, modelSort, entitySort, false);

                return modelQuery.ToArray();
            }

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<StandardClassification>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.StandardClassifications.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Binding

        public static Guid? SelectFirstCategoryIdentifier(Guid assetIdentifier)
        {
            return BindFirst(x => (Guid?)x.CategoryIdentifier, x => x.StandardIdentifier == assetIdentifier);
        }

        public static StandardClassification SelectFirst(Expression<Func<StandardClassification, bool>> filter, params Expression<Func<StandardClassification, object>>[] includes) =>
            StandardClassificationReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<StandardClassification> Select(
            Expression<Func<StandardClassification, bool>> filter,
            params Expression<Func<StandardClassification, object>>[] includes) => StandardClassificationReadHelper.Instance.Select(filter, includes);

        public static IReadOnlyList<StandardClassification> Select(
            Expression<Func<StandardClassification, bool>> filter,
            string sortExpression,
            params Expression<Func<StandardClassification, object>>[] includes) => StandardClassificationReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T[] Bind<T>(
            Expression<Func<StandardClassification, T>> binder,
            Expression<Func<StandardClassification, bool>> filter,
            string modelSort = null,
            string entitySort = null) => StandardClassificationReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<StandardClassification, T>> binder,
            Expression<Func<StandardClassification, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            StandardClassificationReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<StandardClassification, bool>> filter) => StandardClassificationReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<StandardClassification, bool>> filter) =>
            StandardClassificationReadHelper.Instance.Exists(filter);

        #endregion

        #region SELECT

        public static List<StandardClassification> SelectByAssetIdentifier(Guid assetIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.StandardClassifications.Where(ac => ac.StandardIdentifier == assetIdentifier).ToList();
            }
        }

        public static List<StandardClassification> Select(Guid? assetIdentifier, Guid? categoryIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.StandardClassifications.AsQueryable();

                if (assetIdentifier != null) query = query.Where(ac => ac.StandardIdentifier == assetIdentifier.Value);
                if (categoryIdentifier != null) query = query.Where(ac => ac.CategoryIdentifier == categoryIdentifier.Value);

                return query.ToList();
            }
        }

        public static bool Exists(Guid? assetIdentifier, Guid? categoryIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.StandardClassifications.AsQueryable();

                if (assetIdentifier != null) query = query.Where(ac => ac.StandardIdentifier == assetIdentifier.Value);
                if (categoryIdentifier != null) query = query.Where(ac => ac.CategoryIdentifier == categoryIdentifier.Value);

                return query.Any();
            }
        }

        #endregion
    }
}
