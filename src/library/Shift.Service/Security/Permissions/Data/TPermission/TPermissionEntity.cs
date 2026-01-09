using Shift.Service.Directory;

namespace Shift.Service.Security;

public partial class TPermissionEntity
{
    public GroupEntity? Group { get; set; }
    public OrganizationEntity? Organization { get; set; }

    public Guid GroupIdentifier { get; set; }
    public Guid ObjectIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid? PermissionGrantedBy { get; set; }
    public Guid PermissionIdentifier { get; set; }

    public bool AllowAdministrate { get; set; }
    public bool AllowConfigure { get; set; }
    public bool AllowCreate { get; set; }
    public bool AllowDelete { get; set; }
    public bool AllowExecute { get; set; }
    public bool AllowRead { get; set; }
    public bool AllowTrialAccess { get; set; }
    public bool AllowWrite { get; set; }

    public string ObjectType { get; set; } = null!;

    public int PermissionMask { get; set; }

    public DateTimeOffset? PermissionGranted { get; set; }
}