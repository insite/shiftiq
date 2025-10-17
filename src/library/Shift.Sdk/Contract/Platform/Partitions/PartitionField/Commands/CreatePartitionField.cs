using System;

namespace Shift.Contract
{
    public class CreatePartitionField
    {
        public Guid SettingIdentifier { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}