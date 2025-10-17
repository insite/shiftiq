using System;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// A comment is a block of text posted about something in the bank. For example, it might be descriptive 
    /// commentary from an instructor building a specification, or it might be feedback from a student on a specific 
    /// question.
    /// </summary>
    [Serializable]
    public class Comment
    {
        /// <summary>
        /// The colored flag assigned to the comment.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FlagType Flag { get; set; }

        /// <summary>
        /// Identifies the type for the Subject of the comment. Type and Subject taken together can be used to find the
        /// thing that the comment is about.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CommentType Type { get; set; }

        /// <summary>
        /// The person who posted the comment.
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        /// The person who revised the comment.
        /// </summary>
        public Guid? Revisor { get; set; }

        /// <summary>
        /// Uniquely identifies the comment.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// The instructor or training provider to whom the comment pertains.
        /// </summary>
        public Guid? Instructor { get; set; }

        /// <summary>
        /// Uniquely identifies the subject of the comment.
        /// </summary>
        public Guid Subject { get; set; }

        /// <summary>
        /// The role of the author posting the comment (Administrator|Candidate).
        /// </summary>
        public string AuthorRole { get; set; }

        /// <summary>
        /// Used by administrators to categorize/classify comments.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The format of the exam event activity to which the comment pertains.
        /// </summary>
        public string EventFormat { get; set; }

        /// <summary>
        /// The body text for the comment itself. (We do NOT need multilingual support here. Authors are expected to 
        /// post comments in whatever language they speak.)
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The date of the exam event activity to which the comment pertains.
        /// </summary>
        public DateTimeOffset? EventDate { get; set; }

        /// <summary>
        /// The date and time the comment was posted.
        /// </summary>
        public DateTimeOffset Posted { get; set; }

        /// <summary>
        /// The date and time the comment was revised.
        /// </summary>
        public DateTimeOffset? Revised { get; set; }

        /// <summary>
        /// If true then a comment is hidden/archived.
        /// </summary>
        public bool IsHidden { get; set; }

        public Comment Clone()
        {
            var clone = new Comment();

            this.ShallowCopyTo(clone);

            return clone;
        }

        public string GetSubjectTitle(BankState bank)
        {
            string result;

            if (Type == CommentType.Bank)
                result = bank?.ToString();
            else if (Type == CommentType.Specification)
                result = bank.FindSpecification(Subject)?.ToString();
            else if (Type == CommentType.Set)
                result = bank.FindSet(Subject)?.ToString();
            else if (Type == CommentType.Criterion)
                result = bank.FindCriterion(Subject)?.ToString();
            else if (Type == CommentType.Form)
                result = bank.FindForm(Subject)?.ToString();
            else if (Type == CommentType.Field)
                result = bank.FindField(Subject)?.Question.ToString();
            else if (Type == CommentType.Question)
                result = bank.FindQuestion(Subject)?.ToString();
            else
                throw new ApplicationError("Unexpected comment type: " + Type.GetName());

            return string.IsNullOrEmpty(result) ? "(Undefined)" : result;
        }
    }
}
