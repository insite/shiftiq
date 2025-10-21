using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class ProfileFilter : Filter
    {
        public Boolean IsHierarchySelect { get; set; }
        public Guid? ProfileOrganizationIdentifier { get; set; }
        public String ProfileVisibility { get; set; }
        public Guid? ParentProfileStandardIdentifier { get; set; }

        public Guid? OrganizationIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? CompaniesForPersonId { get; set; }
        public Guid? ExcludeUserIdentifier { get; set; }
        public Guid? ProfileUserIdentifier { get; set; }
        public String Category { get; set; }
        public String ProfileNumber { get; set; }
        public String ProfileTitle { get; set; }
        public String ProfileDescription { get; set; }
        public Guid? ExcludeProfileStandardIdentifier { get; set; }

        public Guid? AddProfilesFromOrganizationIdentifier { get; set; }

        public ProfileFilter Clone() 
        {
            return (ProfileFilter)MemberwiseClone();
        }
    }
}
