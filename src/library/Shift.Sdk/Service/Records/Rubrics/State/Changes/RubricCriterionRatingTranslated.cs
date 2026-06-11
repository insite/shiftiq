using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class RubricCriterionRatingTranslated : Change
    {
        public Guid RubricRatingId { get; }
        public ContentContainer Content { get; }

        public RubricCriterionRatingTranslated(Guid rubricRatingId, ContentContainer content)
        {
            RubricRatingId = rubricRatingId;
            Content = content;
        }
    }
}
