using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CommentPrivacyChanged : Change
    {
        public Guid Comment { get; set; }
        public bool CommentPrivacy { get; set; }

        public CommentPrivacyChanged(Guid comment, bool commentPrivacy)
        {
            Comment = comment;
            CommentPrivacy = commentPrivacy;
        }
    }
}
