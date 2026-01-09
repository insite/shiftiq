using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class RubricCriterionTranslated : Change
    {
        public Guid RubricCriterionId { get; }
        public ContentContainer Content { get; }

        public RubricCriterionTranslated(Guid rubricCriterionId, ContentContainer content)
        {
            RubricCriterionId = rubricCriterionId;
            Content = content;
        }
    }
}
