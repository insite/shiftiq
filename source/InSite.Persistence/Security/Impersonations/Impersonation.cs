using System;

namespace InSite.Persistence
{
    public class Impersonation
    {
        public Guid ImpersonationIdentifier { get; set; }
        public Guid ImpersonatedUserIdentifier { get; set; }
        public Guid ImpersonatorUserIdentifier { get; set; }
        public DateTimeOffset ImpersonationStarted { get; set; }
        public DateTimeOffset? ImpersonationStopped { get; set; }
    }
}