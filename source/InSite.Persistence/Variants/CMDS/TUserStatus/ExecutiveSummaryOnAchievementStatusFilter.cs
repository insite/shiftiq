using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class ExecutiveSummaryOnAchievementStatusFilter : Filter
    {
        public DateTimeRange AsAt { get; set; } = new DateTimeRange();
        public DateTime? SnapshotAsAt { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public string AchievementType { get; set; }

        public ExecutiveSummaryOnAchievementStatusFilter Clone()
        {
            return (ExecutiveSummaryOnAchievementStatusFilter)MemberwiseClone();
        }
    }
}
