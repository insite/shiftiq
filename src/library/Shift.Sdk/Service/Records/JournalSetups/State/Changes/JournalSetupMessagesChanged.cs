using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupMessagesChanged : Change
    {
        public Guid? ValidatorMessage { get; }
        public Guid? LearnerMessage { get; }
        public Guid? LearnerAddedMessage { get; }

        public JournalSetupMessagesChanged(Guid? validatorMessage, Guid? learnerMessage, Guid? learnerAddedMessage)
        {
            ValidatorMessage = validatorMessage;
            LearnerMessage = learnerMessage;
            LearnerAddedMessage = learnerAddedMessage;
        }
    }
}
