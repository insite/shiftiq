using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressPointsChanged : Change
    {
        public ProgressPointsChanged(decimal? points, decimal? maxPoints, DateTimeOffset? graded)
        {
            Points = points;
            MaxPoints = maxPoints;
            Graded = graded;
        }

        public decimal? Points { get; set; }
        public decimal? MaxPoints { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
