using Shift.Common;

namespace Shift.Contract
{
    public class CountPartitionFields : Query<int>, IPartitionFieldCriteria
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}