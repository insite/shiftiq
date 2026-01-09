using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    [Serializable]
    public class PersonCommentSummaryFilter : Filter
    {
        public DateTime? UtcPostedSince { get; set; }
        public DateTime? UtcPostedBefore { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorName { get; set; }
        public string Keyword { get; set; }
        public InclusionType? ReminderInclusion { get; set; }
        public Guid? UserIdentifier { get; set; }
        public InclusionType? DescriptionInclusion { get; set; }
    }
}
