using System;

namespace InSite.Domain.Issues
{
    /// <summary>
    /// An attachment is an uploaded file with metadata.
    /// </summary>
    [Serializable]
    public class IssueAttachment
    {
        /// <summary>
        /// Name of the attachment.
        /// </summary>
        public Guid Name { get; set; }

        /// <summary>
        /// Path of the attachment.
        /// </summary>
        public Guid Path { get; set; }

        /// <summary>
        /// Extension of the attachment.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Posted time of the attachment.
        /// </summary>
        public DateTimeOffset Posted { get; set; }

        /// <summary>
        /// Identifier of an attachment poster.
        /// </summary>
        public Guid Poster { get; set; }
    }
}
