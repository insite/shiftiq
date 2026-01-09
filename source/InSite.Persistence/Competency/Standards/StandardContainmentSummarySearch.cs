using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence
{
    public static class StandardContainmentSummarySearch
    {
        private class ReadHelper : ReadHelper<StandardContainmentSummary>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<StandardContainmentSummary>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.StandardContainmentSummaries.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static T[] Bind<T>(
            Expression<Func<StandardContainmentSummary, T>> binder,
            Expression<Func<StandardContainmentSummary, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
    }
}
