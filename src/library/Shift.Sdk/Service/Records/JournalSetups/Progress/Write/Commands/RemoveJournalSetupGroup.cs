using System;

using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.Application.JournalSetups.Write
{
    public class RemoveJournalSetupGroup : Command
    {
        public Guid Group { get; }
        public JournalSetupUserRole Role { get; }

        public RemoveJournalSetupGroup(Guid journalSetup, Guid group, JournalSetupUserRole role)
        {
            AggregateIdentifier = journalSetup;
            Group = group;
            Role = role;
        }
    }
}
