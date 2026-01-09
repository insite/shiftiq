using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class ChangeProgressText : Command
    {
        public ChangeProgressText(Guid progress, string text, DateTimeOffset? graded)
        {
            AggregateIdentifier = progress;
            Text = text;
            Graded = graded;
        }

        public string Text { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
