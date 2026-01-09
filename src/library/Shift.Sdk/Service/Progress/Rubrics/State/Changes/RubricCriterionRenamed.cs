using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionRenamed : Change
    {
        public Guid RubricCriterionId { get; set; }
        public string CriterionTitle { get; set; }

        public RubricCriterionRenamed(Guid rubricCriterionId, string criterionTitle)
        {
            RubricCriterionId = rubricCriterionId;
            CriterionTitle = criterionTitle;
        }
    }
}
