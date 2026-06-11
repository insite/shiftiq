using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeComment : Command
    {
        public Guid Comment { get; }
        public string Text { get; }
        public DateTimeOffset Revised { get; }
        public bool IsPrivate { get; }

        public ChangeComment(Guid journal, Guid comment, string text, DateTimeOffset revised, bool isPrivate)
        {
            AggregateIdentifier = journal;
            Comment = comment;
            Text = text;
            Revised = revised;
            IsPrivate = isPrivate;
        }
    }
}
