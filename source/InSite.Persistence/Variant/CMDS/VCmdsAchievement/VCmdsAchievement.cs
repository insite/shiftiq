using System;
using System.Collections.Generic;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievement
    {
        public Guid AchievementIdentifier { get; set; }
        public string AchievementDescription { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public bool AchievementIsEnabled { get; set; }
        public bool AchievementAllowSelfDeclared { get; set; }
        public string ValidForUnit { get; set; }
        public string Visibility { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public int? ValidForCount { get; set; }

        public virtual ICollection<VCmdsAchievementCategory> Categories { get; set; } = new HashSet<VCmdsAchievementCategory>();
        public virtual ICollection<VCmdsAchievementCompetency> Competencies { get; set; } = new HashSet<VCmdsAchievementCompetency>();
        public virtual ICollection<VCmdsCredential> Credentials { get; set; } = new HashSet<VCmdsCredential>();
        public virtual ICollection<VCmdsAchievementDepartment> Departments { get; set; } = new HashSet<VCmdsAchievementDepartment>();
        public virtual ICollection<VCmdsAchievementOrganization> Organizations { get; set; } = new HashSet<VCmdsAchievementOrganization>();
    }
}
