using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class AddressSearch
    {
        #region Classes

        public class GroupAddress
        {
            public string AddressType { get; set; }
            public Address Address { get; set; }
        }

        public class UserAddress
        {
            public string AddressType { get; set; }
            public Address Address { get; set; }
            public string GmapsLink { get; set; }
        }

        #endregion

        public static List<string> SelectGroupCountries(Guid organizationId, string search)
        {
            using (var db = new InternalDbContext())
                return db.VOrganizationGroupAddresses.Where(x => x.OrganizationIdentifier == organizationId && !string.IsNullOrEmpty(x.Country) && (search == null || x.Country.StartsWith(search)))
                    .Select(x => x.Country)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        public static List<string> SelectPersonCountries(Guid organizationId, string search)
        {
            using (var db = new InternalDbContext())
                return db.VOrganizationPersonAddresses.Where(x => x.OrganizationIdentifier == organizationId && !string.IsNullOrEmpty(x.Country) && (search == null || x.Country.StartsWith(search)))
                    .Select(x => x.Country)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        public static List<string> SelectGroupProvinces(Guid organizationId, string search)
        {
            using (var db = new InternalDbContext())
                return db.VOrganizationGroupAddresses.Where(x => x.OrganizationIdentifier == organizationId && !string.IsNullOrEmpty(x.Province) && (search == null || x.Province.StartsWith(search)))
                    .Select(x => x.Province)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        public static List<string> SelectPersonProvinces(Guid organizationId, string search)
        {
            using (var db = new InternalDbContext())
                return db.VOrganizationPersonAddresses.Where(x => x.OrganizationIdentifier == organizationId && !string.IsNullOrEmpty(x.Province) && (search == null || x.Province.StartsWith(search)))
                    .Select(x => x.Province)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        public static List<string> SelectGroupCities(Guid organizationId, string search)
        {
            using (var db = new InternalDbContext())
                return db.VOrganizationGroupAddresses.Where(x => x.OrganizationIdentifier == organizationId && !string.IsNullOrEmpty(x.City) && (search == null || x.City.StartsWith(search)))
                    .Select(x => x.City)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        public static List<string> SelectPersonCities(Guid organizationId, ICollection<string> provinces)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VOrganizationPersonAddresses.Where(x => x.OrganizationIdentifier == organizationId && !string.IsNullOrEmpty(x.City)).Distinct();

                if (provinces.IsNotEmpty())
                    query = query.Where(x => provinces.Contains(x.Province));

                return query
                    .Select(x => x.City)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public static int CountCities(Guid organizationId, string search)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(organizationId, search, db).Count();
        }

        public static List<string> SelectCities(Guid organizationId, string search, Paging paging)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(organizationId, search, db)
                    .OrderBy(x => x)
                    .ApplyPaging(paging)
                    .ToList();
            }
        }

        private static IQueryable<string> CreateQuery(Guid organizationId, string search, InternalDbContext db)
        {
            return db.Addresses
                .Where(x => x.City != null && (search == null || x.City.StartsWith(search)) &&
                    (
                       x.BillingPersons.Any(a => a.OrganizationIdentifier == organizationId)
                       || x.HomePersons.Any(a => a.OrganizationIdentifier == organizationId)
                       || x.ShippingPersons.Any(a => a.OrganizationIdentifier == organizationId)
                       || x.WorkPersons.Any(a => a.OrganizationIdentifier == organizationId)
                   ))
                .Select(x => x.City)
                .Distinct();
        }

        public static Address Select(Guid address)
        {
            using (var db = new InternalDbContext())
                return db.Addresses.AsNoTracking().FirstOrDefault(x => x.AddressIdentifier == address);
        }

        public static string[] SelectProvinces()
        {
            const string query = @"
select distinct Province from locations.Address where Province is not null and Province <> '' order by Province
";
            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<string>(query).ToArray();
        }

        public static string[] SelectCities()
        {
            const string query = @"
select distinct City from locations.Address where City is not null and City <> '' order by City
";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<string>(query).ToArray();
        }

        private static UserAddress[] ToArray(params UserAddress[] addresses)
            => addresses.Where(x => x != null && x.Address != null).OrderBy(x => x.AddressType).ThenBy(x => x.Address.Street1).ThenBy(x => x.Address.City).ToArray();

        private static GroupAddress[] ToArray(params GroupAddress[] addresses)
            => addresses.Where(x => x != null && x.Address != null).OrderBy(x => x.AddressType).ThenBy(x => x.Address.Street1).ThenBy(x => x.Address.City).ToArray();
    }
}
