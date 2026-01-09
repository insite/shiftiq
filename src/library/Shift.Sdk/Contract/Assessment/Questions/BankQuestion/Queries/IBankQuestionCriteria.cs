using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankQuestionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? BankIdentifier { get; set; }
        Guid? CompetencyIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? ParentQuestionIdentifier { get; set; }
        Guid? QuestionSourceIdentifier { get; set; }
        Guid? RubricIdentifier { get; set; }
        Guid? SetIdentifier { get; set; }

        string LastChangeType { get; set; }
        string LastChangeUser { get; set; }
        string QuestionAssetNumber { get; set; }
        string QuestionCode { get; set; }
        string QuestionCondition { get; set; }
        string QuestionFlag { get; set; }
        string QuestionLikeItemGroup { get; set; }
        string QuestionReference { get; set; }
        string QuestionSourceAssetNumber { get; set; }
        string QuestionTag { get; set; }
        string QuestionTags { get; set; }
        string QuestionText { get; set; }
        string QuestionType { get; set; }

        int? BankIndex { get; set; }
        int? BankSubIndex { get; set; }
        int? QuestionDifficulty { get; set; }
        int? QuestionTaxonomy { get; set; }

        DateTimeOffset? LastChangeTime { get; set; }
        DateTimeOffset? QuestionFirstPublished { get; set; }
    }
}