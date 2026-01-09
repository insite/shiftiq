using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class ProgramSearch
    {
        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        public static int CountPrograms(TProgramFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public static TProgram GetProgram(Guid id, params Expression<Func<TProgram, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.TPrograms
                    .AsNoTracking()
                    .Where(x => x.ProgramIdentifier == id)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public static List<TProgram> GetPrograms(IEnumerable<Guid> ids, params Expression<Func<TProgram, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.TPrograms
                    .Where(x => ids.Contains(x.ProgramIdentifier))
                    .ApplyIncludes(includes);

                return query.ToList();
            }
        }

        public static List<TProgram> GetPrograms(TProgramFilter filter, params Expression<Func<TProgram, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderBy(x => x.ProgramCode).ThenBy(x => x.ProgramName);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public static List<TProgram> SelectProgramsByCategory(Guid categoryId)
        {
            using (var db = CreateContext())
            {
                var query = db.TProgramCategories
                    .AsQueryable()
                    .Where(x => x.ItemIdentifier == categoryId)
                    .Select(x => x.Program)
                    .OrderBy(x => x.ProgramName);

                return query.ToList();
            }
        }

        private static IQueryable<TProgram> CreateQuery(TProgramFilter filter, InternalDbContext db)
        {
            var query = db.TPrograms.AsQueryable();

            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.CatalogIdentifier.HasValue)
                query = query.Where(x => x.CatalogIdentifier == filter.CatalogIdentifier);

            if (filter.ProgramCode.HasValue())
                query = query.Where(x => x.ProgramCode.Contains(filter.ProgramCode));

            if (filter.ProgramName.HasValue())
                query = query.Where(x => x.ProgramName.Contains(filter.ProgramName));

            if (filter.ProgramDescription.HasValue())
                query = query.Where(x => x.ProgramDescription.Contains(filter.ProgramDescription));

            return query;
        }
    }
}
