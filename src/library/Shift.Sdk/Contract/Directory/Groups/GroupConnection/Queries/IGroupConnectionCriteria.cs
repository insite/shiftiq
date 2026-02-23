using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupConnectionCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }
    }
}