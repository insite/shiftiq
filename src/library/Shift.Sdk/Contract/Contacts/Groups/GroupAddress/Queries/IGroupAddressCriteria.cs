using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupAddressCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }
    }
}
