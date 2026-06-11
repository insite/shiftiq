using System;

namespace InSite.Persistence
{
    public class ImpersonationSummary
    {
        public String ImpersonatedUserFullName { get; set; }
        public Guid ImpersonatedUserIdentifier { get; set; }
        public DateTimeOffset ImpersonationStarted { get; set; }
        public DateTimeOffset? ImpersonationStopped { get; set; }
        public Boolean ImpersonatorAccessGrantedToCmds { get; set; }
        public String ImpersonatorUserFullName { get; set; }
        public Guid ImpersonatorUserIdentifier { get; set; }
        public bool ImpersonatorIsCloaked { get; set; }
    }
}
