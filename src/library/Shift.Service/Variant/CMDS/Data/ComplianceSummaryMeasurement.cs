namespace Shift.Service.Variant.CMDS;

public class ComplianceSummaryMeasurement
{
    public int Key { get; set; }
    public string Name { get; set; } = null!;
    public decimal? Score { get; set; }

    public int Required { get; set; }
    public int Satisfied { get; set; }
    public int? Expired { get; set; }
    public int? NotCompleted { get; set; }
    public int? NotApplicable { get; set; }
    public int? NeedsTraining { get; set; }
    public int? SelfAssessed { get; set; }
    public int? Submitted { get; set; }
    public int? Validated { get; set; }
}
