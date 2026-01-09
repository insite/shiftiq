using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class CommentRetracted : Change
    {
        public Guid Comment { get; set; }

        public CommentRetracted(Guid comment)
        {
            Comment = comment;
        }
    }
}
