namespace Shift.Service.Security;

public partial class TUserFieldEntity
{
    public Guid OrganizationIdentifier { get; set; }
    public Guid SettingIdentifier { get; set; }
    public Guid UserIdentifier { get; set; }

    public string Name { get; set; } = null!;
    public string ValueJson { get; set; } = null!;
    public string ValueType { get; set; } = null!;
}