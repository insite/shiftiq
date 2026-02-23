using System;

namespace Shift.Contract
{
    public partial class UserFieldModel
    {
        public Guid OrganizationId { get; set; }
        public Guid SettingId { get; set; }
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string ValueJson { get; set; }
        public string ValueType { get; set; }
    }
}