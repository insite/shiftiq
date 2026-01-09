using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitTimestampsModified : Change
    {
        public Guid UnitId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public CourseUnitTimestampsModified(Guid unitId, Guid createdBy, DateTimeOffset created, Guid modifiedBy, DateTimeOffset modified)
        {
            UnitId = unitId;
            CreatedBy = createdBy;
            Created = created;
            ModifiedBy = modifiedBy;
            Modified = modified;
        }
    }
}
