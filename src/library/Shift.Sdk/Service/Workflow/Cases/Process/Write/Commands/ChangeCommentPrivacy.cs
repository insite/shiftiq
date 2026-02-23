using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ChangeCommentPrivacy : Command
    {
        public Guid Comment { get; set; }
        public bool CommentPrivacy { get; set; }

        public ChangeCommentPrivacy(Guid aggregate, Guid comment, bool commentPrivacy)
        {
            AggregateIdentifier = aggregate;
            Comment = comment;
            CommentPrivacy = commentPrivacy;
        }
    }
}
