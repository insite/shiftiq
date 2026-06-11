using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class CommentVisibilityChanged : Change
    {
        public Guid Comment { get; set; }
        public bool IsHidden { get; set; }

        public CommentVisibilityChanged(Guid comment, bool isHidden)
        {
            Comment = comment;
            IsHidden = isHidden;
        }
    }
}
