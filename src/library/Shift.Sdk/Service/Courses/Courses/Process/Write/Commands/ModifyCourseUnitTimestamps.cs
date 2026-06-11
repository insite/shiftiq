using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseUnitTimestamps : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public ModifyCourseUnitTimestamps(Guid courseId, Guid unitId, Guid createdBy, DateTimeOffset created, Guid modifiedBy, DateTimeOffset modified)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            CreatedBy = createdBy;
            Created = created;
            ModifiedBy = modifiedBy;
            Modified = modified;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null)
                return false;

            course.Apply(new CourseUnitTimestampsModified(UnitId, CreatedBy, Created, ModifiedBy, Modified));
            return true;
        }
    }
}
