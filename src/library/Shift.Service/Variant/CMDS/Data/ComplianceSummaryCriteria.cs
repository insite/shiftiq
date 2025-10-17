namespace Shift.Service.Variant.CMDS;

public class ComplianceSummaryCriteria
{
    public Guid[] Departments { get; set; } = null!;
    public Guid[] Learners { get; set; } = null!;
    public int[] Measurements { get; set; } = null!;
    public int ProfileCondition { get; set; }
    public bool LearnerMustHaveProfile { get; set; }
    public bool? DepartmentEmployment { get; set; }
    public bool? OrganizationEmployment { get; set; }
    public bool? DataAccess { get; set; }
    public string? ZeroRequirementsProgressDisplay { get; set; }
}