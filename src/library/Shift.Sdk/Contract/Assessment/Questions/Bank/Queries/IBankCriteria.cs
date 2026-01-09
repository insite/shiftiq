using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? DepartmentIdentifier { get; set; }
        Guid? FrameworkIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }

        bool? IsActive { get; set; }

        string BankEdition { get; set; }
        string BankLevel { get; set; }
        string BankName { get; set; }
        string BankStatus { get; set; }
        string BankTitle { get; set; }
        string BankType { get; set; }
        string LastChangeType { get; set; }
        string LastChangeUser { get; set; }

        int? AssetNumber { get; set; }
        int? AttachmentCount { get; set; }
        int? BankSize { get; set; }
        int? CommentCount { get; set; }
        int? FormCount { get; set; }
        int? OptionCount { get; set; }
        int? QuestionCount { get; set; }
        int? SetCount { get; set; }
        int? SpecCount { get; set; }

        DateTimeOffset? LastChangeTime { get; set; }
    }
}