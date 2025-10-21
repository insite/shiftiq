using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsCredentialFilter2
    {
        public string AchievementLabel { get; set; }
        public string CredentialStatus { get; set; }
        public DateTimeOffset? CredentialGranted { get; set; }
        public bool IsSuccess { get; set; }
    }
}
