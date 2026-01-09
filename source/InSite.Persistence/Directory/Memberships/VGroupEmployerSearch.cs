using System;
using System.Linq;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static partial class VGroupEmployerSearch
    {
        public static VGroupEmployer SelectByGroup(Guid vGroupEmployerId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VGroupEmployers.FirstOrDefault(x => x.GroupIdentifier == vGroupEmployerId);
            }
        }

        public static VGroupEmployer SelectByUser(Guid entityId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VGroupEmployers.FirstOrDefault(x => x.UserIdentifier == entityId);
            }
        }

        public static int Count(VGroupEmployerFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db).Count();
            }
        }

        public static SearchResultList Select(VGroupEmployerFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 5 * 60;

                var list = CreateQuery(filter, db)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter)
                    .ToSearchResult();

                return list;
            }
        }

        private static IQueryable<VGroupEmployer> CreateQuery(VGroupEmployerFilter filter, InternalDbContext db)
        {
            var query = db.VGroupEmployers.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            query = query.Where(x => !String.IsNullOrEmpty(x.GroupName));

            if (filter.ParentGroupIdentifier.HasValue)
            {
                var children = db.QGroupConnections
                    .Where(x => x.ParentGroupIdentifier == filter.ParentGroupIdentifier)
                    .Select(x => x.ChildGroupIdentifier)
                    .ToArray();

                query = query.Where(x => children.Any(y => y == x.GroupIdentifier));
            }

            if (filter.GroupDepartmentIdentifiers.IsNotEmpty())
            {
                query = query.Where(x =>
                     db.Memberships.Any(y =>
                         filter.GroupDepartmentIdentifiers.Contains(y.GroupIdentifier)
                        && y.UserIdentifier == x.UserIdentifier
                         )
                     );
            }

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.Address.IsNotEmpty())
                query = query.Where(x => x.AddressLine.Contains(filter.Address));

            if (filter.Country.IsNotEmpty())
                query = query.Where(x => x.AddressCountry.Contains(filter.Country));

            if (filter.Province.IsNotEmpty())
                query = query.Where(x => x.AddressProvince.Contains(filter.Province));

            if (filter.City.IsNotEmpty())
                query = query.Where(x => x.AddressCity.Contains(filter.City));

            if (filter.Email.IsNotEmpty())
                query = query.Where(x => x.Email.Contains(filter.Email));

            if (filter.Phone.IsNotEmpty())
                query = query.Where(x => x.GroupPhone.Contains(filter.Phone));

            if (filter.EmployerName.IsNotEmpty())
                query = query.Where(x => x.GroupName.Contains(filter.EmployerName));

            if (filter.EmployerContactName.IsNotEmpty())
                query = query.Where(x => x.ContactFullName.Contains(filter.EmployerContactName));

            if (filter.EmployerSize.IsNotEmpty())
                query = query.Where(x => x.GroupSize.Contains(filter.EmployerSize));

            if (filter.Industry.IsNotEmpty())
                query = query.Where(x => x.GroupIndustry.Contains(filter.Industry));

            if (filter.Sector.IsNotEmpty())
                query = query.Where(x => x.CompanySector.Contains(filter.Sector));

            if (filter.IsApproved.HasValue)
            {
                if (filter.IsApproved.Value)
                    query = query.Where(x => x.Approved != null);
                else
                    query = query.Where(x => x.Approved == null);
            }

            if (filter.DateRegisteredSince.HasValue)
                query = query.Where(x => x.GroupCreated >= filter.DateRegisteredSince.Value);

            if (filter.DateRegisteredBefore.HasValue)
                query = query.Where(x => x.GroupCreated < filter.DateRegisteredBefore.Value);

            return query;
        }
    }
}
