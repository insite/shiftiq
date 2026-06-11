using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionDescribed : Change
    {
        public Guid RubricCriterionId { get; set; }
        public string CriterionDescription { get; set; }

        public RubricCriterionDescribed(Guid rubricCriterionId, string criterionDescription)
        {
            RubricCriterionId = rubricCriterionId;
            CriterionDescription = criterionDescription;
        }
    }
}
