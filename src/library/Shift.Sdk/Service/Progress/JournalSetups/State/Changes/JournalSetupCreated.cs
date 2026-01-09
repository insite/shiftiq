using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupCreated : Change
    {
        public Guid Tenant { get; }
        public string Name { get; }

        public JournalSetupCreated(Guid tenant, string name)
        {
            Tenant = tenant;
            Name = name;
        }
    }
}
