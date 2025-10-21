using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class CompanyFilter : Filter
    {
        public Guid? UserIdentifier { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }
        public Guid? AchievementForEmployeeId { get; set; }
        public Guid[] IncludeOrganizationIdentifiers { get; set; }
        public string Name { get; set; }
        public string RequireMembershipForUserEmail { get; set; }
        public InclusionType Archived { get; set; }

        public CompanyFilter()
        {
            Archived = InclusionType.Exclude;
        }

        public CompanyFilter Clone()
        {
            return (CompanyFilter)MemberwiseClone();
        }
    }
}
