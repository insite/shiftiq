using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectOrganizations : Query<IEnumerable<OrganizationModel>>, IOrganizationCriteria
    {
        public string CompanyNameContains { get; set; }
        public string OrganizationCode { get; set; }
    }
}