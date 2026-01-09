using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseCommentModified : Change
    {
        public Guid Comment { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Flag { get; set; }
        public string Tag { get; set; }

        public Guid Revisor { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? ResolvedBy { get; set; }

        public DateTimeOffset Revised { get; set; }
        public DateTimeOffset? Flagged { get; set; }
        public DateTimeOffset? Submitted { get; set; }
        public DateTimeOffset? Resolved { get; set; }

        public CaseCommentModified(Guid comment, string text, string category, string flag,
            Guid revisor, Guid? assignedTo, Guid? resolvedBy, string subCategory, string tag,
            DateTimeOffset revised, DateTimeOffset? flagged, DateTimeOffset? submitted, DateTimeOffset? resolved)
        {
            Comment = comment;

            Text = text;
            Category = category;
            SubCategory= subCategory;
            Flag = flag;
            Tag = tag;

            Revisor = revisor;
            AssignedTo = assignedTo;
            ResolvedBy = resolvedBy;

            Revised = revised;
            Flagged = flagged;
            Submitted = submitted;
            Resolved = resolved;
        }
    }
}
