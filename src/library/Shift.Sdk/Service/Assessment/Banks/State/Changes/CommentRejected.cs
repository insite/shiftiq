using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class CommentRejected : Change
    {
        public Guid Comment { get; set; }

        public CommentRejected(Guid comment)
        {
            Comment = comment;
        }
    }
}
