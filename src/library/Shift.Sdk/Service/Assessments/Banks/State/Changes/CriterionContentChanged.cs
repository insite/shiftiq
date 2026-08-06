using System;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class CriterionContentChanged : Change
    {
        public Guid Criterion { get; set; }
        public ContentExamCriterion Content { get; set; }

        public CriterionContentChanged(Guid criterion, ContentExamCriterion content)
        {
            Criterion = criterion;
            Content = content;
        }
    }
}
