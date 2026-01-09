using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankSpecificationCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? BankIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }

        string CalcDisclosure { get; set; }
        string SpecConsequence { get; set; }
        string SpecName { get; set; }
        string SpecType { get; set; }

        int? CriterionAllCount { get; set; }
        int? CriterionPivotCount { get; set; }
        int? CriterionTagCount { get; set; }
        int? SpecAsset { get; set; }
        int? SpecFormCount { get; set; }
        int? SpecFormLimit { get; set; }
        int? SpecQuestionLimit { get; set; }

        decimal? CalcPassingScore { get; set; }
    }
}