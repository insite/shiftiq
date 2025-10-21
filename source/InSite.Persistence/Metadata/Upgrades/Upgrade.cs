using System;

namespace InSite.Persistence
{
    public class Upgrade
    {
        public Guid UpgradeIdentifier { get; set; }
        public string ScriptName { get; set; }
        public DateTimeOffset UtcUpgraded { get; set; }
        public string ScriptData { get; set; }
    }
}
