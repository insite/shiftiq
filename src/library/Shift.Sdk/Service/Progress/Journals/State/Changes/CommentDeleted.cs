using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CommentDeleted : Change
    {
        public Guid Comment { get; }

        public CommentDeleted(Guid comment)
        {
            Comment = comment;
        }
    }
}
