using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrantContactInformationChanged : Change
    {
        public RegistrantChangedField[] ChangedFields { get; set; }

        public RegistrantContactInformationChanged(RegistrantChangedField[] changedFields)
        {
            ChangedFields = changedFields;
        }
    }
}
