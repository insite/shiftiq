using System;

namespace Shift.Sdk.UI
{
    public class BankExportCommentItem
    {
        public string CommentPosted { get; set; }
        public string CommentRevised { get; set; }
        public string CommentText { get; set; }
        public string CommentCategory { get; set; }
        public string CommentFlag { get; set; }
        public string CommentAuthorType { get; set; }

        public string AuthorName { get; set; }
        public string RevisorName { get; set; }

        public string ContainerSubtype { get; set; }
        public string ContainerDescription { get; set; }
        public Guid BankIdentifier { get; set; }
        public Guid? FieldIdentifier { get; set; }
        public Guid? FormIdentifier { get; set; }
        public Guid? QuestionIdentifier { get; set; }
        public Guid? SpecificationIdentifier { get; set; }

        public string InstructorName { get; set; }
        public string EventDate { get; set; }
        public string EventFormat { get; set; }

        public string QuestionSequenceInBank { get; set; }
        public string QuestionAsset { get; set; }
        public string QuestionFormName { get; set; }
        public string QuestionSequenceInForm { get; set; }
        public string QuestionCompetencyArea { get; set; }
        public string QuestionCompetencyItem { get; set; }
    }
}