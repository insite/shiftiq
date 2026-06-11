using System;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public class RunMonthlyJobs
    {
        public void Execute(string organizationCode = null)
        {
            var exclusions = new string[] { "abc", "archive", "cmds", "ogiu", "testinc" };

            var organizations = OrganizationSearch.SelectAll()
                .Where(x => x.AccountClosed == null
                         && x.AccountStatus == AccountStatus.Opened
                         && !exclusions.Any(exclusion => exclusion == x.OrganizationCode))
                .ToList();

            if (organizationCode.IsNotEmpty())
                organizations = organizations.Where(x => x.Code.StartsWith(organizationCode)).ToList();

            var n = organizations.Count;

            Console.WriteLine($"Start calculating monthly statistics for {n} organizations.");

            for (var i = 0; i < n; i++)
            {
                var organization = organizations[i];

                ServiceLocator.IdentityService.SetCurrentOrganization(organization.OrganizationIdentifier);

                Console.WriteLine($"  Calculating statistics for {organization.CompanyName}.");

                TUserStatusStore.CreateSnapshot(organization.OrganizationIdentifier);
            }
        }
    }
}