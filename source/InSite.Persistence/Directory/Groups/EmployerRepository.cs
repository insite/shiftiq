using System.Linq;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class EmployerRepository
    {
        public static int CountByEmployerFilter(EmployerFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByEmployerFilter(filter, db).Count();
            }
        }

        public static SearchResultList SelectByEmployerFilter(EmployerFilter filter)
        {
            var sortExpression = "EmployerGroupName";

            if (!string.IsNullOrEmpty(filter.SortByColumn))
                sortExpression = filter.SortByColumn;

            using (var db = new InternalDbContext())
            {
                return CreateQueryByEmployerFilter(filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        private static IQueryable<Employer> CreateQueryByEmployerFilter(EmployerFilter filter, InternalDbContext db)
        {
            var query = db.Employers.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.EmployerOrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.EmployerName))
                query = query.Where(x => x.EmployerGroupName.Contains(filter.EmployerName));

            return query;
        }
    }
}