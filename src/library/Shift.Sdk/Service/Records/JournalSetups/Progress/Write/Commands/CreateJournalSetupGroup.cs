using System;

using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.Application.JournalSetups.Write
{
    public class CreateJournalSetupGroup : Command
    {
        public Guid Group { get; }
        public JournalSetupUserRole Role { get; }

        public CreateJournalSetupGroup(Guid journalSetup, Guid group, JournalSetupUserRole role)
        {
            AggregateIdentifier = journalSetup;
            Group = group;
            Role = role;
        }
    }
}
