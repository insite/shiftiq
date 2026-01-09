namespace Shift.Service.Progress;

public partial class PeriodEntity
{
    public Guid OrganizationIdentifier { get; set; }
    public Guid PeriodIdentifier { get; set; }

    public string PeriodName { get; set; } = null!;

    public DateTimeOffset PeriodEnd { get; set; }
    public DateTimeOffset PeriodStart { get; set; }
}