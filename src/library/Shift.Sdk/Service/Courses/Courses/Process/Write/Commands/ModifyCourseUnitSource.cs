using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseUnitSource : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public Guid? Source { get; set; }

        public ModifyCourseUnitSource(Guid courseId, Guid unitId, Guid? source)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            Source = source;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || unit.SourceIdentifier == Source)
                return false;

            course.Apply(new CourseUnitSourceModified(UnitId, Source));
            return true;
        }
    }
}
