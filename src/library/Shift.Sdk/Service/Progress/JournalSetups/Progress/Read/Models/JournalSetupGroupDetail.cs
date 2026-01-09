using System;

namespace InSite.Application.Records.Read
{
    public class JournalSetupGroupDetail
    {
        public Guid JournalSetupIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public DateTimeOffset Created { get; set; }

        public string GroupName { get; set; }
        public int MembershipCount { get; set; }
    }
}
