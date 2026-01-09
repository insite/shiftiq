using System;

namespace InSite.Persistence
{
    public class TGroupSetting
    {
        public Guid SettingIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

    }
}
