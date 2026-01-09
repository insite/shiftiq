using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CompetencyRequirementDeleted : Change
    {
        public Guid Competency { get; }

        public CompetencyRequirementDeleted(Guid competency)
        {
            Competency = competency;
        }
    }
}
