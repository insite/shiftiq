using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Banks.Read
{
    [Serializable]
    public class QBankCommentFilter : Filter
    {
        public Guid? AuthorIdentifier { get; set; }
        public Guid? BankIdentifier { get; set; }
        public Guid? SubjectIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public FlagType? CommentFlag { get; set; }
        public CommentType? CommentType { get; set; }

        public string CommentText { get; set; }

        public DateTimeOffset? CommentPostedSince { get; set; }
        public DateTimeOffset? CommentPostedBefore { get; set; }

        public QBankCommentFilter Clone()
        {
            return (QBankCommentFilter)MemberwiseClone();
        }
    }
}
