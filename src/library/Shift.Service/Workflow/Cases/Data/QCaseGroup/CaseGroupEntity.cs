namespace Shift.Service.Workflow;

public partial class CaseGroupEntity
{
    public Guid GroupIdentifier { get; set; }
    public Guid CaseIdentifier { get; set; }
    public Guid JoinIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }

    public string CaseRole { get; set; } = null!;
}