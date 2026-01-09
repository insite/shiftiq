using Shift.Common;

namespace Shift.Contract
{
    public interface IPartitionFieldCriteria
    {
        QueryFilter Filter { get; set; }

        string SettingName { get; set; }

        string SettingValue { get; set; }
    }
}