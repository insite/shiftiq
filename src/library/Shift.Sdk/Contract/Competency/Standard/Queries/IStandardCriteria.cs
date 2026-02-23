using Shift.Common;

namespace Shift.Contract
{
    public interface IStandardCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        string ContentTitle { get; set; }
        string StandardType { get; set; }
    }
}