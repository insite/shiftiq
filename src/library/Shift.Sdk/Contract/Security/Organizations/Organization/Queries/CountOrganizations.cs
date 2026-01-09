using Shift.Common;

namespace Shift.Contract
{
    public class CountOrganizations : Query<int>, IOrganizationCriteria
    {
        public string CompanyNameContains { get; set; }
        public string OrganizationCode { get; set; }
    }
}