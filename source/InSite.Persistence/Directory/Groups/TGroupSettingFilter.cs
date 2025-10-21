using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TGroupSettingFilter : Filter
    {
        public Guid? SettingIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }

        public string SettingName { get; set; }
        public string SettingValue { get; set; }

    }
}
