using System;

namespace Shift.Contract
{
    public class ModifyGroupField
    {
        public Guid GroupId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid SettingId { get; set; }

        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}