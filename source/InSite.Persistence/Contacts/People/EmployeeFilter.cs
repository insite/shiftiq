using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class EmployeeFilter : Filter
    {
        public string SortByColumn { get; set; }

        public Guid? OrganizationIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }

        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeJobTitle { get; set; }
        public string EmployeeGender { get; set; }
        public DateTime? EmployeeJoinedSince { get; set; }
        public DateTime? EmployeeJoinedBefore { get; set; }
        public DateTime? EmployeeEndedSince { get; set; }
        public DateTime? EmployeeEndedBefore { get; set; }

        public string EmployerNumber { get; set; }
        public Guid? EmployerDistrictIdentifier { get; set; }
        public Guid? MembershipGroupIdentifier { get; set; }

        public string[] MembershipStatuses { get; set; }
    }
}