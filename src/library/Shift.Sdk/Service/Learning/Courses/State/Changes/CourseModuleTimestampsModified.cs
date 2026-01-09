using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleTimestampsModified : Change
    {
        public Guid ModuleId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public CourseModuleTimestampsModified(Guid moduleId, Guid createdBy, DateTimeOffset created, Guid modifiedBy, DateTimeOffset modified)
        {
            ModuleId = moduleId;
            CreatedBy = createdBy;
            Created = created;
            ModifiedBy = modifiedBy;
            Modified = modified;
        }
    }
}
