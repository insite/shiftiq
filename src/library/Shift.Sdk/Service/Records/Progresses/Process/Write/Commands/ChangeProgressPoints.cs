using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class ChangeProgressPoints : Command
    {
        public ChangeProgressPoints (Guid progress, decimal? points, decimal? maxPoints, DateTimeOffset? graded)
        {
            AggregateIdentifier = progress;
            Points = points;
            MaxPoints = maxPoints;
            Graded = graded;
        }

        public decimal? Points { get; set; }
        public decimal? MaxPoints { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
