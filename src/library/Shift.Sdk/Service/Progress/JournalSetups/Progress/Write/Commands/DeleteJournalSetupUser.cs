using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.JournalSetups.Write
{
    public class DeleteJournalSetupUser : Command
    {
        public Guid User { get; }
        public JournalSetupUserRole Role { get; }

        public DeleteJournalSetupUser(Guid journalSetup, Guid user, JournalSetupUserRole role)
        {
            AggregateIdentifier = journalSetup;
            User = user;
            Role = role;
        }
    }
}
