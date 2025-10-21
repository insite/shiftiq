using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ProfileCertification
    {
        public String AuthorityName { get; set; }
        public DateTimeOffset? DateGranted { get; set; }
        public DateTimeOffset? DateRequested { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
