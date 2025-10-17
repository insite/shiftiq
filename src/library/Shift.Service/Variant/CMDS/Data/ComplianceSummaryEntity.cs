namespace Shift.Service.Variant.CMDS;

public class ComplianceSummaryEntity
{
    public Guid SummaryIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public string CompanyName { get; set; } = default!;
    public Guid DepartmentIdentifier { get; set; }
    public string DepartmentName { get; set; } = default!;
    public Guid UserIdentifier { get; set; }
    public string UserFullName { get; set; } = default!;
    public Guid? PrimaryProfileIdentifier { get; set; }
    public string? PrimaryProfileNumber { get; set; }
    public string? PrimaryProfileTitle { get; set; }
    public int Sequence { get; set; }
    public string Heading { get; set; } = default!;
    public int Required { get; set; }
    public int Satisfied { get; set; }
    public decimal Score { get; set; }
    public int? Expired { get; set; }
    public int? NotCompleted { get; set; }
    public int? NotApplicable { get; set; }
    public int? NeedsTraining { get; set; }
    public int? SelfAssessed { get; set; }
    public int? Submitted { get; set; }
    public int? Validated { get; set; }
}
