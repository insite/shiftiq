using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? AdministratorUserIdentifier { get; set; }
        Guid? EmployerGroupIdentifier { get; set; }
        Guid? CaseClosedBy { get; set; }
        Guid? CaseOpenedBy { get; set; }
        Guid? CaseStatusIdentifier { get; set; }
        Guid? LastChangeUser { get; set; }
        Guid? LawyerUserIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? OwnerUserIdentifier { get; set; }
        Guid? ResponseSessionIdentifier { get; set; }
        Guid? TopicUserIdentifier { get; set; }

        string CaseDescription { get; set; }
        string CaseSource { get; set; }
        string CaseStatusCategory { get; set; }
        string CaseTitle { get; set; }
        string CaseType { get; set; }
        string LastChangeType { get; set; }

        int? AttachmentCount { get; set; }
        int? CommentCount { get; set; }
        int? GroupCount { get; set; }
        int? CaseNumber { get; set; }
        int? PersonCount { get; set; }

        DateTimeOffset? CaseClosed { get; set; }
        DateTimeOffset? CaseOpened { get; set; }
        DateTimeOffset? CaseReported { get; set; }
        DateTimeOffset? CaseStatusEffective { get; set; }
        DateTimeOffset? LastChangeTime { get; set; }
    }
}