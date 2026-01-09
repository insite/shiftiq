using System;

using InSite.Domain.Organizations;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class DepartmentFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public OrganizationList Organizations { get; set; }
        public Guid? ExcludeDepartmentIdentifier { get; set; }

        public Guid? ExcludeUserIdentifier { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }
        public Guid? AchievementForEmployeeId { get; set; }
        public Guid? UserIdentifier { get; set; }
        public string[] RoleType { get; set; }
        public Guid? ProfileStandardIdentifier { get; set; }

        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public Guid? DivisionIdentifier { get; set; }
        public DateTimeOffset? CreatedSince { get; set; }
        public DateTimeOffset? CreatedBefore { get; set; }
        public string DepartmentLabel { get; set; }
        public string CompanyName { get; set; }

        public DepartmentFilter Clone()
        {
            return (DepartmentFilter)MemberwiseClone();
        }
    }
}
