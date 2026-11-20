using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class ExecutiveSummaryOnCompetencyStatusFilter : Filter
    {
        public DateTimeRange AsAt { get; set; } = new DateTimeRange();
        public DateTime? SnapshotAsAt { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? DivisionIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public string Criticality { get; set; }

        public ExecutiveSummaryOnCompetencyStatusFilter Clone()
        {
            return (ExecutiveSummaryOnCompetencyStatusFilter)MemberwiseClone();
        }
    }
}
