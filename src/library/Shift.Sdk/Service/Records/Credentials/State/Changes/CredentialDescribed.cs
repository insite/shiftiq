using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    [Obsolete]
    public class CredentialDescribed : Change
    {
        public Guid User { get; set; }
        public string Description { get; set; }

        public CredentialDescribed(Guid user, string description)
        {
            User = user;
            Description = description;
        }
    }

    public class CredentialDescribed2 : Change
    {
        public string Description { get; set; }

        public CredentialDescribed2(string description)
        {
            Description = description;
        }
    }
}