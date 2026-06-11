using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityTimestamps : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public ModifyCourseActivityTimestamps(Guid courseId, Guid activityId, Guid createdBy, DateTimeOffset created, Guid modifiedBy, DateTimeOffset modified)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            CreatedBy = createdBy;
            Created = created;
            ModifiedBy = modifiedBy;
            Modified = modified;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null)
                return false;

            course.Apply(new CourseActivityTimestampsModified(ActivityId, CreatedBy, Created, ModifiedBy, Modified));
            return true;
        }
    }
}
