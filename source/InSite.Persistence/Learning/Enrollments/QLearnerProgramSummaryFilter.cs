using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class QLearnerProgramSummaryFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }

        public string SnapshotCriteriaType { get; set; }
        public DateTimeOffsetRange AsAt { get; set; } = new DateTimeOffsetRange();

        public QLearnerProgramSummaryFilter()
        {
            AsAt = new DateTimeOffsetRange();
        }

        public QLearnerProgramSummaryFilter Clone()
        {
            return (QLearnerProgramSummaryFilter)MemberwiseClone();
        }
    }
}
