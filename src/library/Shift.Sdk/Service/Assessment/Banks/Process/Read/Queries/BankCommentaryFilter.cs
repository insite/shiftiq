using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Banks.Read
{
    [Serializable]
    public class BankCommentaryFilter : Filter
    {
        public FlagType? CommentFlag { get; set; }

        public Guid? OrganizationIdentifier { get; set; }

        public Guid[] BankIdentifier { get; set; }
        public Guid[] FieldIdentifier { get; set; }
        public Guid[] FormIdentifier { get; set; }
        public Guid[] QuestionIdentifier { get; set; }
        public Guid[] SpecificationIdentifier { get; set; }

        public string EventFormat { get; set; }
        public string AuthorRole { get; set; }
        public string CommentText { get; set; }
        public string[] CommentCategory { get; set; }
        public string[] AttemptTag { get; set; }
        public Guid? AttemptRegistrationEventIdentifier { get; set; }

        public DateTimeOffsetRange CommentPosted { get; set; } = new DateTimeOffsetRange();

        public BankCommentaryFilter Clone()
        {
            return (BankCommentaryFilter)MemberwiseClone();
        }
    }
}
