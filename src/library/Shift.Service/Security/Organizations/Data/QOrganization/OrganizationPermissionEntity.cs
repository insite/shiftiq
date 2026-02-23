namespace Shift.Service.Security;

public class OrganizationPermissionEntity
{
    public Guid OrganizationId { get; set; }
    public string AccessGranted { get; set; } = null!;
    public string AccessDenied { get; set; } = null!;
    public string AccessGrantedToActions { get; set; } = null!;
}