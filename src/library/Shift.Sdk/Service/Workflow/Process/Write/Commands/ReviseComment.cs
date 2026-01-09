using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ReviseComment : Command
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

        public ReviseComment(Guid aggregate, Guid comment, string text, string category, string flag,
            Guid revisor, 
            DateTimeOffset revised, Guid? assignedTo, Guid? resolvedBy, string subCategory, string tag,
            DateTimeOffset? flagged, DateTimeOffset? submitted, DateTimeOffset? resolved)
        {
            AggregateIdentifier = aggregate;
            Comment = comment;
            Text = text;
            Category = category;
            SubCategory = subCategory;
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
