using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseTimestampsModified : Change
    {
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public CourseTimestampsModified(Guid createdBy, DateTimeOffset created, Guid modifiedBy, DateTimeOffset modified)
        {
            CreatedBy = createdBy;
            Created = created;
            ModifiedBy = modifiedBy;
            Modified = modified;
        }
    }
}
