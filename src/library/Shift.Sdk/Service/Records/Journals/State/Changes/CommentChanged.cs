using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CommentChanged : Change
    {
        public Guid Comment { get; }
        public string Text { get; }
        public DateTimeOffset Revised { get; }
        public bool IsPrivate { get; }

        public CommentChanged(Guid comment, string text, DateTimeOffset revised, bool isPrivate)
        {
            Comment = comment;
            Text = text;
            Revised = revised;
            IsPrivate = isPrivate;
        }
    }
}
