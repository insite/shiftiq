using System;

namespace InSite.Persistence
{
    public class VCandidateComment
    {
        public Guid? AuthorUserIdentifier { get; set; }
        public Guid? CandidateUserIdentifier { get; set; }
        public Guid CommentIdentifier { get; set; }

        public string CommentText { get; set; }
        public string ContainerType { get; set; }

        public DateTimeOffset? CommentIsFlagged { get; set; }

        public DateTimeOffset CommentModified { get; set; }

        public string AuthorName { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual User Author { get; set; }
        public virtual User Candidate { get; set; }
    }
}