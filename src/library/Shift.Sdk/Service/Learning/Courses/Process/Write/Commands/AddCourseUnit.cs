using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class AddCourseUnit : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public int UnitAsset { get; set; }
        public string UnitName { get; set; }
        public ContentContainer UnitContent { get; set; }

        public AddCourseUnit(Guid courseId, Guid unitId, int unitAsset, string unitName, ContentContainer unitContent)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            UnitAsset = unitAsset;
            UnitName = unitName;
            UnitContent = unitContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            course.Apply(new CourseUnitAdded(UnitId, UnitAsset, UnitName, UnitContent));
            return true;
        }
    }
}
