namespace Shift.Service.Assessment;

public class BankSpecificationEntity
{
    public Guid BankIdentifier { get; set; }
    public string CalcDisclosure { get; set; } = default!;
    public decimal CalcPassingScore { get; set; }
    public int? CriterionAllCount { get; set; }
    public int? CriterionTagCount { get; set; }
    public int? CriterionPivotCount { get; set; }
    public int SpecAsset { get; set; }
    public string SpecConsequence { get; set; } = default!;
    public int SpecFormCount { get; set; }
    public int SpecFormLimit { get; set; }
    public Guid SpecIdentifier { get; set; }
    public string SpecName { get; set; } = default!;
    public int SpecQuestionLimit { get; set; }
    public string SpecType { get; set; } = default!;
    public Guid OrganizationIdentifier { get; set; }
}
