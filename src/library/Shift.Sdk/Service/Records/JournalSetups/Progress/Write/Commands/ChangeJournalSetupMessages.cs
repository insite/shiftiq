using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupMessages : Command
    {
        public Guid? ValidatorMessage { get; }
        public Guid? LearnerMessage { get; }
        public Guid? LearnerAddedMessage { get; }

        public ChangeJournalSetupMessages(Guid journalSetupIdentifier, Guid? validatorMessage, Guid? learnerMessage, Guid? learnerAddedMessage)
        {
            AggregateIdentifier = journalSetupIdentifier;
            ValidatorMessage = validatorMessage;
            LearnerMessage = learnerMessage;
            LearnerAddedMessage = learnerAddedMessage;
        }
    }
}
