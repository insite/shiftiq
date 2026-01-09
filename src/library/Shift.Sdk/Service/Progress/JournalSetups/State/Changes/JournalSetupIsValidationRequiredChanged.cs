using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupIsValidationRequiredChanged : Change
    {
        public bool IsValidationRequired { get; }

        public JournalSetupIsValidationRequiredChanged(bool isValidationRequired)
        {
            IsValidationRequired = isValidationRequired;
        }
    }
}
