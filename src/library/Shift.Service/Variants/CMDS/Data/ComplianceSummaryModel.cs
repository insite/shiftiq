using Shift.Common;

namespace Shift.Service.Variant.CMDS;

public class ComplianceSummaryModel
{
    public Model Department { get; set; } = null!;

    public Model Learner { get; set; } = null!;

    public Model PrimaryProfile { get; set; } = null!;

    public ComplianceSummaryMeasurement Measurement { get; set; } = null!;
}
