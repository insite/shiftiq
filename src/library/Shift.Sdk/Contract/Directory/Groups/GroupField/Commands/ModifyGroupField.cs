using System;

namespace Shift.Contract
{
    public class ModifyGroupField
    {
        public Guid GroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid SettingIdentifier { get; set; }

        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}