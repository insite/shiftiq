using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeCommentAuthorRole : Command
    {
        public Guid Comment { get; set; }
        public string AuthorRole { get; set; }

        public ChangeCommentAuthorRole(Guid bank, Guid comment, string authorRole)
        {
            AggregateIdentifier = bank;
            Comment = comment;
            AuthorRole = authorRole;
        }
    }
}
