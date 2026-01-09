using Shift.Common;

namespace Shift.Contract
{
    public interface IOrganizationCriteria
    {
        QueryFilter Filter { get; set; }

        string CompanyNameContains { get; set; }
        string OrganizationCode { get; set; }
    }
}