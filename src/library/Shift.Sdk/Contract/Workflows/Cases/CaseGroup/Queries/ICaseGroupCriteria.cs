using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseGroupCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }
    }
}
