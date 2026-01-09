using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class AuthorComment : Command
    {
        public Guid Comment { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Flag { get; set; }
        public string Tag { get; set; }

        public Guid Author { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? ResolvedBy { get; set; }
        public string AuthorRole { get; set; }

        public DateTimeOffset Posted { get; set; }
        public DateTimeOffset? Flagged { get; set; }
        public DateTimeOffset? Submitted { get; set; }
        public DateTimeOffset? Resolved { get; set; }

        public AuthorComment(Guid aggregate, Guid comment, string text, string category, string flag,
            Guid author, string authorRole, Guid? assignedTo,Guid? resolvedBy, string subCategory,string tag,
            DateTimeOffset posted, DateTimeOffset? flagged, DateTimeOffset? submitted, DateTimeOffset? resolved)
        {
            AggregateIdentifier = aggregate;
            Comment = comment;
            Text = text;
            Category = category;
            SubCategory = subCategory;
            Flag = flag;
            Tag = tag;

            Author = author;
            AssignedTo = assignedTo;
            ResolvedBy = resolvedBy;
            AuthorRole = authorRole;

            Posted = posted;
            Flagged = flagged;
            Submitted = submitted;
            Resolved = resolved;
        }
    }
}
