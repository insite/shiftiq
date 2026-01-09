using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventCommentDeleted : Change
    {
        public Guid CommentIdentifier { get; set; }

        public EventCommentDeleted(Guid comment)
        {
            CommentIdentifier = comment;
        }
    }
}
