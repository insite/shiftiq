using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? AdministratorUserId { get; set; }
        Guid? EmployerGroupId { get; set; }
        Guid? CaseClosedBy { get; set; }
        Guid? CaseOpenedBy { get; set; }
        Guid? CaseStatusId { get; set; }
        Guid? LastChangeUser { get; set; }
        Guid? LawyerUserId { get; set; }
        Guid? OwnerUserId { get; set; }
        Guid? ResponseSessionId { get; set; }
        Guid? TopicUserId { get; set; }

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