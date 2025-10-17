using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchOrganizations : Query<IEnumerable<OrganizationMatch>>, IOrganizationCriteria
    {
        public string CompanyNameContains { get; set; }
        public string OrganizationCode { get; set; }
    }
}