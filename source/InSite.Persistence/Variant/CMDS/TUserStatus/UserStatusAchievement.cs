using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserStatusAchievement
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public string AchievementType { get; set; }
        public string AchievementTitle { get; set; }
        public string ValidationStatus { get; set; }
        public int CountCP { get; set; }
        public int CountEX { get; set; }
        public int CountNA { get; set; }
        public int CountNC { get; set; }
        public int CountNT { get; set; }
        public int CountRQ { get; set; }
        public int CountSA { get; set; }
        public int CountSV { get; set; }
        public int CountVA { get; set; }
        public int CountVN { get; set; }
    }
}
