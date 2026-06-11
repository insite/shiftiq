using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICourseCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? ClosedSince { get; set; }
        DateTimeOffset? ClosedBefore { get; set; }
        DateTimeOffset? CreatedSince { get; set; }
        DateTimeOffset? CreatedBefore { get; set; }
        DateTimeOffset? ModifiedSince { get; set; }
        DateTimeOffset? ModifiedBefore { get; set; }
        bool? IsPublished { get; set; }
    }
}