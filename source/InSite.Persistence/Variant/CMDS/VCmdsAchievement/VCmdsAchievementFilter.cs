using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class VCmdsAchievementFilter : Filter
    {
        public Guid? CategoryIdentifier { get; set; }

        public bool? AllowSelfDeclared { get; set; }
        public bool? ExcludeHidden { get; set; }
        public bool GlobalOrCompanySpecific { get; set; }
        public bool? IsTimeSensitive { get; set; }

        public Guid? DepartmentIdentifier { get; set; }
        public Guid? ExcludeAchievementIdentifier { get; set; }
        public Guid? AchievementOrganizationIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string AchievementCategory { get; set; }
        public string AchievementType { get; set; }
        public string AchievementVisibility { get; set; }
        public string OrganizationCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public VCmdsAchievementFilter Clone()
        {
            return (VCmdsAchievementFilter)MemberwiseClone();
        }
    }
}
