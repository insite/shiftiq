using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class AddCourseUnitPrerequisite : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public Prerequisite Prerequisite { get; set; }

        public AddCourseUnitPrerequisite(Guid courseId, Guid unitId, Prerequisite prerequisite)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            Prerequisite = prerequisite;
        }


        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null)
                return false;

            course.Apply(new CourseUnitPrerequisiteAdded(UnitId, Prerequisite));
            return true;
        }
    }
}
