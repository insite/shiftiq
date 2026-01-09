using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseUnitPrerequisite : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public Guid PrerequisiteId { get; set; }

        public RemoveCourseUnitPrerequisite(Guid courseId, Guid unitId, Guid prerequisiteId)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            PrerequisiteId = prerequisiteId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || unit.Prerequisites.Find(x => x.Identifier == PrerequisiteId) == null)
                return false;

            course.Apply(new CourseUnitPrerequisiteRemoved(UnitId, PrerequisiteId));
            return true;
        }
    }
}
