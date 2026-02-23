using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class CommentAuthorRoleChanged : Change
    {
        public Guid Comment { get; set; }
        public string AuthorRole { get; set; }

        public CommentAuthorRoleChanged(Guid comment, string authorRole)
        {
            Comment = comment;
            AuthorRole = authorRole;
        }
    }
}
