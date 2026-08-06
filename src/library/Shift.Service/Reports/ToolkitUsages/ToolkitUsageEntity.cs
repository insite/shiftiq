namespace Shift.Service.Reports;

public class ToolkitUsageEntity
{
    public Guid ToolkitUsageIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid UserIdentifier { get; set; }
    public string ToolkitName { get; set; } = default!;
    public int TotalVisitCount { get; set; }
    public decimal UsageScore { get; set; }
    public DateTimeOffset LastVisited { get; set; }
    public DateTimeOffset Calculated { get; set; }
}