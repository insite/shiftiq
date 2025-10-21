using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class TUserStatusFilter : Filter
    {
        public bool Enabled { get; set; } = true;

        public Guid[] UserIdentifier { get; set; }
        public string UserName { get; set; }
        public int[] ExcludeAchievementTypes { get; set; }
        public int[] ExcludeStandardTypes { get; set; }
        public string ItemName { get; set; }
        public decimal? ScoreFrom { get; set; }
        public decimal? ScoreThru { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public string OrganizationName { get; set; }
        public Guid[] Departments { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentRole { get; set; }
        public string DepartmentLabel { get; set; }
        public DateTimeOffsetRange AsAtRange { get; set; } = new DateTimeOffsetRange();
        public DateTimeOffset? AsAt { get; set; }

        public TUserStatusFilter Clone()
        {
            return (TUserStatusFilter)MemberwiseClone();
        }
    }
}
