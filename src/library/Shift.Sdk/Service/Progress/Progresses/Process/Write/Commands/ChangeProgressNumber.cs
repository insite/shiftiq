using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class ChangeProgressNumber : Command
    {
        public ChangeProgressNumber(Guid progress, decimal? number, DateTimeOffset? graded)
        {
            AggregateIdentifier = progress;
            Number = number;
            Graded = graded;
        }

        public decimal? Number { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
