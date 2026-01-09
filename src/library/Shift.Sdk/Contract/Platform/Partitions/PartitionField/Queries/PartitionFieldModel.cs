using System;

namespace Shift.Contract
{
    public partial class PartitionFieldModel
    {
        public Guid SettingIdentifier { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}