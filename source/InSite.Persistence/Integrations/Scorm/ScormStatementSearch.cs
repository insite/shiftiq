using System;
using System.Linq;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class ScormStatementSearch
    {
        public static int Count(ScormStatementFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db).Count();
            }
        }

        public static SearchResultList Select(ScormStatementFilter filter)
        {
            var orderBy = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "StatementTimestamp DESC";

            using (var db = new InternalDbContext())
            {
                var list = CreateQuery(filter, db)
                    .OrderBy(orderBy)
                    .ApplyPaging(filter)
                    .ToSearchResult();

                return list;
            }
        }

        public static TScormStatement Select(Guid statement)
        {
            using (var db = new InternalDbContext())
            {
                return db.TScormStatements.FirstOrDefault(x => x.StatementIdentifier == statement);
            }
        }

        private static IQueryable<TScormStatement> CreateQuery(ScormStatementFilter filter, InternalDbContext db)
        {
            var query = db.TScormStatements.AsNoTracking().AsQueryable();

            if (filter.RegistrationIdentifier.HasValue)
                query = query.Where(x => x.RegistrationIdentifier == filter.RegistrationIdentifier);

            return query;
        }
    }
}
