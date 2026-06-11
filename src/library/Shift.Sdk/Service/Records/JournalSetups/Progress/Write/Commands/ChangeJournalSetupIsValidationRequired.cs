using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupIsValidationRequired : Command
    {
        public bool IsValidationRequired { get; }

        public ChangeJournalSetupIsValidationRequired(Guid journalSetup, bool isValidationRequired)
        {
            AggregateIdentifier = journalSetup;
            IsValidationRequired = isValidationRequired;
        }
    }
}
