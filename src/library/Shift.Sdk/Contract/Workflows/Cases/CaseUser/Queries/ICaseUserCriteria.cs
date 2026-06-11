using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseUserCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }
    }
}
