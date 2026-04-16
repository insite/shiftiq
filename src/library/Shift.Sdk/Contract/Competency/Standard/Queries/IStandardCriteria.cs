using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IStandardCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? ParentStandardId { get; set; }
        Guid[] ParentStandardIds { get; set; }
        Guid[] StandardIds { get; set; }

        string ContentTitle { get; set; }
        string StandardType { get; set; }
    }
}