using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseUnitContent : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public ContentContainer UnitContent { get; set; }

        public ModifyCourseUnitContent(Guid courseId, Guid unitId, ContentContainer unitContent)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            UnitContent = unitContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || unit.Content.IsEqual(UnitContent))
                return false;

            course.Apply(new CourseUnitContentModified(UnitId, UnitContent));
            return true;
        }
    }
}
