using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class CompleteProgress : Command
    {
        public CompleteProgress(
            Guid progress,
            DateTimeOffset? completed,
            decimal? percent,
            bool? pass,
            int? elapsedMinutes,
            int? elapsedSeconds = 0
            )
        {
            AggregateIdentifier = progress;
            Completed = completed;
            Percent = percent;
            Pass = pass;
            ElapsedMinutes = elapsedMinutes;
            ElapsedSeconds = elapsedSeconds;
        }

        public DateTimeOffset? Completed { get; set; }
        public decimal? Percent { get; set; }
        public bool? Pass { get; set; }
        public int? ElapsedMinutes { get; set; }
        public int? ElapsedSeconds { get; set; }
    }
}
