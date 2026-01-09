using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseUnitAdded : Change
    {
        public Guid UnitId { get; set; }
        public int UnitAsset { get; set; }
        public string UnitName { get; set; }
        public ContentContainer UnitContent { get; set; }

        public CourseUnitAdded(Guid unitId, int unitAsset, string unitName, ContentContainer unitContent)
        {
            UnitId = unitId;
            UnitAsset = unitAsset;
            UnitName = unitName;
            UnitContent = unitContent;
        }
    }
}
