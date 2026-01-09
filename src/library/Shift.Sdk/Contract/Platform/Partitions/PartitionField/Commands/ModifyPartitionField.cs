using System;

namespace Shift.Contract
{
    public class ModifyPartitionField
    {
        public Guid SettingIdentifier { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}