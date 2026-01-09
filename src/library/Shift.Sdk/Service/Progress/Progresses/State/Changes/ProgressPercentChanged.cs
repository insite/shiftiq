using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressPercentChanged : Change
    {
        public ProgressPercentChanged(decimal? percent, DateTimeOffset? graded)
        {
            Percent = percent;
            Graded = graded;
        }

        public decimal? Percent { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
