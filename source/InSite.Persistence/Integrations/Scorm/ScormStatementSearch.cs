using System;
using System.Collections.Generic;
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

        public static TScormStatement Select(string hook, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.TScormStatements.FirstOrDefault(x => x.Registration.ScormPackageHook == hook && x.Registration.LearnerIdentifier == user);
            }
        }

        public static List<TScormStatement> SelectByOrganizationIdentifier(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.TScormStatements.Where(x => x.Registration.OrganizationIdentifier == organization);
                return query.OrderByDescending(x => x.StatementTimestamp).ToList();
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
