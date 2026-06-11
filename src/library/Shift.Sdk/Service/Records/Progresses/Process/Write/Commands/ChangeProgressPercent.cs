using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class ChangeProgressPercent : Command
    {
        public ChangeProgressPercent(Guid progress, decimal? percent, DateTimeOffset? graded)
        {
            AggregateIdentifier = progress;
            Percent = percent;
            Graded = graded;
        }

        public decimal? Percent { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
