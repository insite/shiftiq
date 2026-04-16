namespace Shift.Service.Timeline;

public class ChangeEntity
{
	public Guid AggregateIdentifier { get; set; }
	public int AggregateVersion { get; set; }
	public Guid OriginOrganization { get; set; }
	public Guid OriginUser { get; set; }
	public DateTimeOffset ChangeTime { get; set; }
	public string ChangeType { get; set; } = null!;
	public string ChangeData { get; set; } = null!;
}
