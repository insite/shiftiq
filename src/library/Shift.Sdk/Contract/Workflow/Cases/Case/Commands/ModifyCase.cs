using System;

namespace Shift.Contract
{
    public class ModifyCase
    {
        public Guid? AdministratorUserId { get; set; }
        public Guid? EmployerGroupId { get; set; }
        public Guid? CaseClosedBy { get; set; }
        public Guid CaseId { get; set; }
        public Guid? CaseOpenedBy { get; set; }
        public Guid? CaseStatusId { get; set; }
        public Guid LastChangeUser { get; set; }
        public Guid? LawyerUserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? OwnerUserId { get; set; }
        public Guid? ResponseSessionId { get; set; }
        public Guid? TopicUserId { get; set; }

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