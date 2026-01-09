using System;

namespace Shift.Contract
{
    public class CreateCase
    {
        public Guid? AdministratorUserIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? CaseClosedBy { get; set; }
        public Guid CaseIdentifier { get; set; }
        public Guid? CaseOpenedBy { get; set; }
        public Guid? CaseStatusIdentifier { get; set; }
        public Guid LastChangeUser { get; set; }
        public Guid? LawyerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? OwnerUserIdentifier { get; set; }
        public Guid? ResponseSessionIdentifier { get; set; }
        public Guid? TopicUserIdentifier { get; set; }

        public string CaseDescription { get; set; }
        public string CaseSource { get; set; }
        public string CaseStatusCategory { get; set; }
        public string CaseTitle { get; set; }
        public string CaseType { get; set; }
        public string LastChangeType { get; set; }

        public int AttachmentCount { get; set; }
        public int CommentCount { get; set; }
        public int GroupCount { get; set; }
        public int CaseNumber { get; set; }
        public int PersonCount { get; set; }

        public DateTimeOffset? CaseClosed { get; set; }
        public DateTimeOffset CaseOpened { get; set; }
        public DateTimeOffset? CaseReported { get; set; }
        public DateTimeOffset? CaseStatusEffective { get; set; }
        public DateTimeOffset LastChangeTime { get; set; }
    }
}