using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.JournalSetups.Write
{
    public class AddJournalSetupUser : Command
    {
        public Guid User { get; }
        public JournalSetupUserRole Role { get; }

        public AddJournalSetupUser(Guid journalSetup, Guid user, JournalSetupUserRole role)
        {
            AggregateIdentifier = journalSetup;
            User = user;
            Role = role;
        }
    }
}
