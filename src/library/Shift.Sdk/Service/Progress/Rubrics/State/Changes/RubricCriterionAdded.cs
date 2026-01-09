using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionAdded : Change
    {
        public Guid RubricCriterionId { get; set; }
        public string CriterionTitle { get; set; }
        public bool IsRange { get; set; }
        public int? CriterionSequence { get; set; }

        public RubricCriterionAdded(Guid rubricCriterionId, string criterionTitle, bool isRange, int? criterionSequence)
        {
            RubricCriterionId = rubricCriterionId;
            CriterionTitle = criterionTitle;
            IsRange = isRange;
            CriterionSequence = criterionSequence;
        }
    }
}
