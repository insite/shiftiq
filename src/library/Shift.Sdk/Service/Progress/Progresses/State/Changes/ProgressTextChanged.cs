using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressTextChanged : Change
    {
        public ProgressTextChanged(string text, DateTimeOffset? graded)
        {
            Text = text;
            Graded = graded;
        }

        public string Text { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
