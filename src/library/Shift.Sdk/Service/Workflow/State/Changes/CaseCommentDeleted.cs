using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseCommentDeleted : Change
    {
        public Guid Comment { get; set; }

        public CaseCommentDeleted(Guid comment)
        {
            Comment = comment;
        }
    }
}
