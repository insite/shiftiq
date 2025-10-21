using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class CmdsPersonFilter : Filter
    {
        public CmdsPersonFilter()
        {
            AccessGrantedToCmds = true;
            EnableIsArchived = true;
            IsArchived = false;
        }

        public bool? AccessGrantedToCmds { get; set; }
        public Guid? ExcludeUserIdentifier { get; set; }
        public RelationCategory? RelationCategory { get; set; }
        public String[] KeyeraRoles { get; set; }
        public Guid? ParentUserIdentifier { get; set; }
        public RelationCategory[] RelationWithParent { get; set; }
        public String Name { get; set; }
        public String NameFilterType { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String EmailWork { get; set; }
        public Boolean? IsApproved { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid[] Departments { get; set; }
        public String[] RoleType { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public Guid? ProfileStandardIdentifier { get; set; }
        public Guid? DepartmentsForParentId { get; set; }
        public Guid? CompaniesForParentId { get; set; }
        public Boolean EnableIsArchived { get; set; }
        public Boolean? IsArchived { get; set; }
        public String EmailStatus { get; set; }
        public Int32? MailingListId { get; set; }
        public Boolean? EmailEnabled { get; set; }
        public string JobDivision { get; set; }
        public String EmployeeType { get; set; }
        public String PersonCode { get; set; }
        public String PersonType { get; set; }
        public string ListStatus { get; set; }

        public CmdsPersonFilter Clone()
        {
            return (CmdsPersonFilter)MemberwiseClone();
        }
    }
}