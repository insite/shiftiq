using System;

namespace Shift.Contract
{
    public partial class UserFieldModel
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid SettingIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string Name { get; set; }
        public string ValueJson { get; set; }
        public string ValueType { get; set; }
    }
}