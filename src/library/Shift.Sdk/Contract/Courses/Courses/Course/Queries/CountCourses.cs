using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCourses : Query<int>, ICourseCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ClosedSince { get; set; }
        public DateTimeOffset? ClosedBefore { get; set; }
        public DateTimeOffset? CreatedSince { get; set; }
        public DateTimeOffset? CreatedBefore { get; set; }
        public DateTimeOffset? ModifiedSince { get; set; }
        public DateTimeOffset? ModifiedBefore { get; set; }
        public bool? IsPublished { get; set; }
    }
}