using System;

namespace InSite.Domain.Issues
{
    /// <summary>
    /// A comment is a block of text posted about issue.
    /// </summary>
    [Serializable]
    public class IssueComment
    {
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
        /// The name of the author posting the comment.
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// The name of the revisor posting the comment.
        /// </summary>
        public string RevisorName { get; set; }

        /// <summary>
        /// The body text for the comment itself. (We do NOT need multilingual support here. Authors are expected to 
        /// post comments in whatever language they speak.)
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The date and time the comment was posted.
        /// </summary>
        public DateTimeOffset Authored { get; set; }

        /// <summary>
        /// The date and time the comment was revised.
        /// </summary>
        public DateTimeOffset? Revised { get; set; }
    }
}
