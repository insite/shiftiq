using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;

namespace InSite.Persistence.Foundation
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query, Expression<Func<T, object>>[] includes)
        {
            if (includes.IsEmpty())
                return query;

            foreach (var i in includes)
            {
                if (i != null)
                    query = query.Include(i);
            }

            return query;
        }
    }
}
