using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupFieldCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }
    }
}
