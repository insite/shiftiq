using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseUnitSequence : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public int UnitSequence { get; set; }

        public ModifyCourseUnitSequence(Guid courseId, Guid unitId, int unitSequence)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            UnitSequence = unitSequence;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || unit.UnitSequence == UnitSequence)
                return false;

            course.Apply(new CourseUnitSequenceModified(UnitId, UnitSequence));
            return true;
        }
    }
}
