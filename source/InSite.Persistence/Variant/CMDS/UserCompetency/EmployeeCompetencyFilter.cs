using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class EmployeeCompetencyFilter : Filter
    {
        public bool ShowValidationHistory { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? UserDepartmentIdentifier { get; set; }
        public string EmployeeDepartmentAssignment { get; set; }
        public Guid? ProfileStandardIdentifier { get; set; }
        public string[] Statuses { get; set; }
        public string[] ExcludeStatuses { get; set; }
        public string Criticality { get; set; }
        public Guid? CategoryIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public string Keyword { get; set; }
        public string SelfAssessmentStatus { get; set; }
        public string Number { get; set; }
        public string NumberOld { get; set; }
        public Guid? ManagerUserIdentifier { get; set; }
        public bool ValidationDateMustBeSet { get; set; }
        public bool ValidationDateMustBeNull { get; set; }
        public bool? IsComplianceRequired { get; set; }
        public bool? IsValidated { get; set; }
    }
}
