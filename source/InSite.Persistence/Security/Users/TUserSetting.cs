using System;

namespace InSite.Persistence
{
    public class TUserSetting
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid SettingIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string Name { get; set; }
        public string ValueType { get; set; }
        public string ValueJson { get; set; }
    }
}
