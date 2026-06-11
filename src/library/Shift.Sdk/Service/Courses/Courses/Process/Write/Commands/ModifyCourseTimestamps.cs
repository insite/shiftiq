using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseTimestamps : Command, IHasRun
    {
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public ModifyCourseTimestamps(Guid courseId, Guid createdBy, DateTimeOffset created, Guid modifiedBy, DateTimeOffset modified)
        {
            AggregateIdentifier = courseId;
            CreatedBy = createdBy;
            Created = created;
            ModifiedBy = modifiedBy;
            Modified = modified;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            course.Apply(new CourseTimestampsModified(CreatedBy, Created, ModifiedBy, Modified));
            return true;
        }
    }
}
