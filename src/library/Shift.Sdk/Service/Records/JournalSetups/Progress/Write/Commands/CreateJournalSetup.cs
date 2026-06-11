using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class CreateJournalSetup : Command
    {
        public Guid Tenant { get; }
        public string Name { get; }

        public CreateJournalSetup(Guid journalSetup, Guid tenant, string name)
        {
            AggregateIdentifier = journalSetup;
            Tenant = tenant;
            Name = name;
        }
    }
}
