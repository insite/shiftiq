using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class TOpportunityCategorySearch
    {
        private class CategoryReadHelper : ReadHelper<TOpportunityCategory>
        {
            public static readonly CategoryReadHelper Instance = new CategoryReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TOpportunityCategory>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TOpportunityCategories.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static IReadOnlyList<TOpportunityCategory> SelectJobCategory(
            Expression<Func<TOpportunityCategory, bool>> filter,
            string sortExpression,
            params Expression<Func<TOpportunityCategory,
            object>>[] includes
            ) =>
            CategoryReadHelper.Instance.Select(filter, sortExpression, includes);
    }
}
