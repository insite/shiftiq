using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressNumberChanged : Change
    {
        public ProgressNumberChanged(decimal? number, DateTimeOffset? graded)
        {
            Number = number;
            Graded = graded;
        }

        public decimal? Number { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
