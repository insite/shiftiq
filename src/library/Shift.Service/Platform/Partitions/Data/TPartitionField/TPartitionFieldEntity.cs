namespace Shift.Service.Security;

public partial class TPartitionFieldEntity
{
    public Guid SettingIdentifier { get; set; }

    public string SettingName { get; set; } = null!;
    public string SettingValue { get; set; } = null!;
}