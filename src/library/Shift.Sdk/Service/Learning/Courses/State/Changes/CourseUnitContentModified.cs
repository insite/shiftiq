using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseUnitContentModified : Change
    {
        public Guid UnitId { get; set; }
        public ContentContainer UnitContent { get; set; }

        public CourseUnitContentModified(Guid unitId, ContentContainer unitContent)
        {
            UnitId = unitId;
            UnitContent = unitContent;
        }
    }
}
