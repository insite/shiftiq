using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemMaxPointsChanged : Change
    {
        public Guid Item { get; set; }
        public decimal? MaxPoints { get; set; }

        public GradeItemMaxPointsChanged(Guid item, decimal? maxPoints)
        {
            Item = item;
            MaxPoints = maxPoints;
        }
    }
}
